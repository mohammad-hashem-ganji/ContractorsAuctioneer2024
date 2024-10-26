using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Models;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
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
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            SignInManager<ApplicationUser> signInManger,
            ApplicationDbContext context,
            IConfiguration configuration,
            JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManger = signInManger;
            _context = context;
            _configuration = configuration;
            _jwtSettings = jwtSettings;
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

        public async Task<Result<ApplicationUser>> AuthenticateAsync(string nCode, string phoneNumber)
        {
            const string key = "ParsianContractorAuthenearproject";
            var user = await _userManager.FindByNameAsync(string.Concat(nCode, key));

            if (user == null || user.PhoneNumber != phoneNumber)
            {
                return new Result<ApplicationUser>()
                    .WithValue(null)
                    .Failure(ErrorMessages.InvalidUserNameOrPassword);
            }

            return new Result<ApplicationUser>()
                .WithValue(user)
                .Success("");
        }
        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {

            
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.DateTime),
                new Claim("FirstName", user.FirstName ?? string.Empty),
                new Claim("LastName", user.LastName ?? string.Empty)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

 


        }

    }
}
