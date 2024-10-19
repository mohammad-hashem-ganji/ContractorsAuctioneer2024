using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsAuctioneer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IAuthService _authService;
        private readonly IVerificationService _verificationService;
        private readonly ILastLoginHistoryService _lastLoginHistoryService;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> usermanager, IAuthService authService, IVerificationService verificationService, ILastLoginHistoryService lastLoginHistoryService)
        {
            _signInManager = signInManager;
            _usermanager = usermanager;
            _authService = authService;
            _verificationService = verificationService;
            _lastLoginHistoryService = lastLoginHistoryService;
        }

    

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _authService.AuthenticateAsync(loginDto.Ncode, loginDto.PhoneNumber);
            if (user.Data == null || user.IsSuccessful == false)
            {
                return BadRequest("کد ملی یا شماره همراه اشتباه است.");
            }
            //var token = await _authService.GenerateJwtTokenAsync(user.Data);
            //return Ok(new { Message = $"Hi {user.Data.UserName}", Token = token });
            if (user.Data.PhoneNumber == null)
            {
                return BadRequest("شماره موبال با کد ملی ثبت شده مطابقت ندارد");
            }
            var verification = await _verificationService
                .GenerateAndSendCodeAsync(user.Data.Id, user.Data.PhoneNumber, CancellationToken.None);
            // CHECK THE ROLE
          
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
            return Ok(new { verification });
        }
        [AllowAnonymous]
        [HttpPost("VerifyTwoFactorCode")]
        public async Task<IActionResult> VerifyTwoFactorCode(GetVerificationCodeDto verificationCode, CancellationToken cancellationToken)
        {          

            var token = await _verificationService.VerifyCodeAsync(verificationCode, cancellationToken);
            if (token is null) return BadRequest("هنگام اجرا مشکلی پیش آمد!");


            return Ok(token);
        }
        [Authorize(Roles ="Contractor")]
        [HttpGet("AutherizeAuthenticatedContractorTest")]
        public async Task<IActionResult> AutherizeAuthenticatedContractorTest()
        {            
            var user = User;         
            var a = 0;
            return Ok("you are autherized");
        }
    }
}

