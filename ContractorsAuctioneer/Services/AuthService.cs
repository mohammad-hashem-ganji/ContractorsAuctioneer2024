using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContractorsAuctioneer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManger;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            SignInManager<ApplicationUser> signInManger,
            ApplicationDbContext context, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManger = signInManger;
            _context = context;
            _configuration = configuration;
        }

    
        public async Task<Result<RegisterResult>> RegisterAsync(string nCode, string phoneNumber, string role)
        {
            const string key = "ParsianContractorAuthenearproject";

            var user = new ApplicationUser
            {
                UserName = string.Concat(nCode, key),
                PhoneNumber = phoneNumber
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                RegisterResult registerResult = new RegisterResult
                {
                    IdentityResult = result,
                    RegisteredUserId = 0,
                };
                return new Result<RegisterResult>()
                    .WithValue(registerResult)
                    .Failure("کاربر ساخته نشد !");
            }
            else
            {

                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);

                if (!addToRoleResult.Succeeded)
                {
                    RegisterResult registerResult = new RegisterResult
                    {
                        IdentityResult = result,
                        RegisteredUserId = 0
                    };
                    return new Result<RegisterResult>()
                        .WithValue(registerResult)
                        .Failure($"  نقش  {role}یافت نشد  ");
                }
                else
                {
                    RegisterResult registerResult = new RegisterResult
                    {
                        IdentityResult = IdentityResult.Success,
                        RegisteredUserId = user.Id
                    };
                    return new Result<RegisterResult>()
                        .WithValue(registerResult)
                        .Success(SuccessMessages.UserRegistered);
                }
            }
        }

        public async Task<Result<UserWithRoleDto>> AuthenticateAsync(string nCode, string phoneNumber)
        {
            const string key = "ParsianContractorAuthenearproject";
            var user = await _userManager.FindByNameAsync(string.Concat(nCode, key));
            var roles = await _userManager.GetRolesAsync(user);
            if (user == null || user.PhoneNumber != phoneNumber)
            {
                return new Result<UserWithRoleDto>()
                    .WithValue(null)
                    .Failure(ErrorMessages.InvalidUserNameOrPassword);
            }
            if (roles == null)
            {
                return new Result<UserWithRoleDto>()
                   .WithValue(null)
                   .Failure(ErrorMessages.InvalidUserNameOrPassword);
            }
            UserWithRoleDto userWithRole = new UserWithRoleDto
            {
                Roles = roles,
                User = user
            };
            return new Result<UserWithRoleDto>()
                .WithValue(userWithRole)
                .Success("");
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.DateTime)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
