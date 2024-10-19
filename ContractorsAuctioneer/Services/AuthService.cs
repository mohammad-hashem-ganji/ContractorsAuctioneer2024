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

        //public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        //{
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var claims = new List<Claim>
        //    {
        //       new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //       new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //       new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.DateTime),
        //       new Claim(JwtRegisteredClaimNames.Name, user.FirsName),
        //       new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),


        //    };
        //    foreach (var role in roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var encryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:EncryptionKey"]));
        //    var encryptingCredentials = new EncryptingCredentials(encryptionKey, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);
        //    var tokenDescriptor = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:ValidIssuer"],
        //        audience: _configuration["Jwt:ValidAudience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(1),
        //        signingCredentials: creds


        //        );
        //    //return new JwtSecurityTokenHandler().WriteToken(token);
        //    // encrypt

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(securityToken);
        //}
        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            // Step 1: Create the claims
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.DateTime),
                new Claim("FirstName", user.FirsName ?? string.Empty),
                new Claim("LastName", user.LastName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Step 2: Create the signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Step 3: Create the encrypting credentials

            var encryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:EncryptionKey"]));
            var encryptingCredentials = new EncryptingCredentials(encryptionKey, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);
            // Step 4: Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = creds,
                EncryptingCredentials = encryptingCredentials,
                Issuer = _configuration["Jwt:ValidIssuer"],
                Audience = _configuration["Jwt:ValidAudience"]
            };

            // Step 5: Create and write the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

    }
}
