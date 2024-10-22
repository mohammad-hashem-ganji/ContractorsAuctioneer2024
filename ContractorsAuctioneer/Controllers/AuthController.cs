using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Services;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsAuctioneer.Controllers
{
    /// <summary>
    /// کنترلر احراز هویت برای مدیریت فرآیندهای ورود و تأیید.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IVerificationService _verificationService;
        private readonly ILastLoginHistoryService _lastLoginHistoryService;

        public AuthController(IAuthService authService, IVerificationService verificationService, ILastLoginHistoryService lastLoginHistoryService)
        {
            _authService = authService;
            _verificationService = verificationService;
            _lastLoginHistoryService = lastLoginHistoryService;
        }

        /// <summary>
        /// احراز هویت کاربر با کد ملی و شماره تلفن و ارسال کد تأیید در صورت موفقیت‌آمیز بودن.
        /// </summary>
        /// <param name="loginDto">اطلاعات ورود کاربر شامل کد ملی و شماره تلفن. <see cref="LoginDto"/>.</param>
        /// <param name="cancellationToken">توکن برای لغو درخواست در صورت نیاز.</param>
        /// <returns>در صورت موفقیت کد تأیید ارسال می‌شود، در غیر این صورت پیام خطا برگردانده می‌شود.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(Result<GetVerificationCodeDto>), StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.AuthenticateAsync(loginDto.Ncode, loginDto.PhoneNumber);
            if (user.Data == null || user.IsSuccessful == false)
            {
                return BadRequest(ErrorMessages.NationalCodeOrPhoneNumberAreWrong);
            }

            if (user.Data.PhoneNumber == null)
            {
                return BadRequest(ErrorMessages.NationalCodeAndPhoneNumberDoesMatch);
            }

            var verification = await _verificationService
                .GenerateAndSendCodeAsync(user.Data.Id, user.Data.PhoneNumber, CancellationToken.None);
            if (!verification.IsSuccessful)
            {
                return BadRequest(verification.Message);
            }

            AddLastLoginHistoryDto lastLogin = new AddLastLoginHistoryDto
            {
                ApplicationUserId = user.Data.Id,
                CreatedAt = DateTime.Now,
                CreatedBy = user.Data.Id,
                LastLoginTime = DateTime.Now
            };

            await _lastLoginHistoryService.AddAsync(lastLogin, cancellationToken);
            return Ok(verification);
        }

        /// <summary>
        /// تأیید کد احراز هویت دو مرحله‌ای و ایجاد توکن در صورت موفقیت‌آمیز بودن.
        /// </summary>
        /// <param name="verificationCode">اطلاعات کد تأیید، شامل کد ملی و شماره تلفن. <see cref="GetVerificationCodeDto"/>.</param>
        /// <param name="cancellationToken">توکن برای لغو درخواست در صورت نیاز.</param>
        /// <returns>در صورت موفقیت توکن بازگردانده می‌شود، در غیر این صورت پیام خطا ارسال می‌شود.</returns>
        [AllowAnonymous]
        [HttpPost("VerifyTwoFactorCode")]
        [ProducesResponseType(typeof(Result<UserWithRoleAndTokenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyTwoFactorCode(GetVerificationCodeDto verificationCode, CancellationToken cancellationToken)
        {
            var token = await _verificationService.VerifyCodeAsync(verificationCode, cancellationToken);
            if (token.Data is null)
            {
                return BadRequest("هنگام اجرا مشکلی پیش آمد!");
            }

            return Ok(new {Token = token});
        }
    }

}

