using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto.Username, registerDto.Password, registerDto.Role);
                if (result.IsSuccessful)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An error occurred while retrieving the requests.",
                        Details = ex.Message
                    });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _authService.AuthenticateAsync(loginDto.Username, loginDto.Password);
            if (user.Data == null || user.IsSuccessful == false)
            {
                return Unauthorized();
            }
            IdentityResult userUpdateResult = await _usermanager.UpdateAsync(user.Data);
            if (!userUpdateResult.Succeeded)
            {
                return BadRequest(userUpdateResult.Errors); 
            }
            var phoneNumber = user.Data.PhoneNumber;
            if (phoneNumber == null)
            {
                return BadRequest("Phone number is not set for this user.");
            }
            var result = await _verificationService
                .GenerateAndSendCodeAsync(user.Data.Id, phoneNumber, CancellationToken.None);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }
            //Add lastLoginHistory
            AddLastLoginHistoryDto lastLogin = new AddLastLoginHistoryDto
            {
                ApplicationUserId = user.Data.Id,
                CreatedAt = DateTime.Now,
                CreatedBy = user.Data.Id,
                LastLoginTime = DateTime.Now
            };
            await _lastLoginHistoryService.AddAsync(lastLogin,cancellationToken);
            return Ok(new { RequiresTwoFactor = true, Message = $"2FA code sent to your phone.{user.Data.PhoneNumber}" });
        }

        [HttpPost("VerifyTwoFactorCode")]
        public async Task<IActionResult> VerifyTwoFactorCode(int userId, string code, CancellationToken cancellationToken)
        {          
            var user = await _usermanager.FindByIdAsync(userId.ToString()); 
            if (user is null) return BadRequest("کاربر یافت نشد!");
            var token = await _authService.GenerateJwtTokenAsync(user);
            if (token is null) return BadRequest("هنگام اجرا مشکلی پیش آمد!");
            IdentityResult userUpdateResult = await _usermanager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded) return BadRequest("هنگام اجرا مشکلی پیش آمد!");//(userUpdateResult.Errors);
            var userProfileDto = new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
            return Ok(new { Token = token, Result = userProfileDto });
        }
        //[Authorize]
        //[HttpGet("AutherizeAuthenticatedUsers")]
        //public async Task<IActionResult> AutherizeAuthenticatedUsers()
        //{
        //    //var login = await _signInManager.PasswordSignInAsync("mamali1", "mamali123@#", true, false);
        //    var user = User.Identity?.IsAuthenticated;
        //    var users = HttpContext.User.Identity;
        //    return Ok("you are autherized");
        //}
    }
}

