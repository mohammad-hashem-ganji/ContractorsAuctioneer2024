using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> usermanager, IAuthService authService)
        {
            _signInManager = signInManager;
            _usermanager = usermanager;
            _authService = authService;
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
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _authService.AuthenticateAsync(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            IdentityResult result = await _usermanager.UpdateAsync(user);
            if (result.Succeeded)
            {

                var token = await _authService.GenerateJwtTokenAsync(user);
                var userProfileDto = new UserProfileDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };

                return Ok(new { Token = token, Result = userProfileDto});
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "User update failed.");
            }
        }
        
        [Authorize]
        [HttpGet("AutherizeAuthenticatedUsers")]
        public async Task<IActionResult> AutherizeAuthenticatedUsers()
        {
            //var login = await _signInManager.PasswordSignInAsync("mamali1", "mamali123@#", true, false);
            var user = User.Identity?.IsAuthenticated;
            var users = HttpContext.User.Identity;
            return Ok("you are autherized");
        }
    }
}
