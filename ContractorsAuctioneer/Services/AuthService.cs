using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
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
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<ApplicationUser> signInManger, ApplicationDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManger = signInManger;
            _context = context;
            _configuration = configuration;
        }

        //var roleExists = await _roleManager.RoleExistsAsync(role);
        //if (!roleExists)
        //{
        //    return IdentityResult.Failed(new IdentityError
        //    {
        //        Code = "InvalidRole",
        //        Description = $"The role '{role}' is invalid."
        //    });
        //}
        public async Task<Result<RegisterResult>> RegisterAsync(string username, string password, string role)
        {


            var user = new ApplicationUser
            {
                UserName = username,
                Email = username
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                RegisterResult registerResult = new RegisterResult
                {
                    IdentityResult = result,
                    RegisteredUserId = 0,
                    IdentityError = result.Errors.ToList()
                };
                var a = 0;
                return new Result<RegisterResult>().WithValue(registerResult).Failure("کاربر ساخته نشد !");
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
                    return new Result<RegisterResult>().WithValue(registerResult).Failure($"  نقش  {role}یافت نشد  ");
                }
                else
                {
                    role = role.ToUpper();
                    //switch (role)
                    //{
                    //    case "CLIENT":
                    //        user.Client = new Client();
                    //        break;

                    //    case "CONTRACTOR":
                    //        user.Contractor = new Contractor();
                    //        break;

                    //    default:
                    //        return 0;
                    //}
                    //_context.Users.Add(user);
                    //await _context.SaveChangesAsync();
                    RegisterResult registerResult = new RegisterResult
                    {
                        IdentityResult = IdentityResult.Success,
                        RegisteredUserId = user.Id
                    };
                    return new Result<RegisterResult>().WithValue(registerResult).Success(SuccessMessages.UserRegistered);
                    

                }
            }
        }

        public async Task<ApplicationUser> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }
            var login = await _signInManger.PasswordSignInAsync(user.UserName, password, true, false);
            if (login.Succeeded)
            {
                return user;
            }
            else return null;
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.DateTime)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
                );

            //return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
