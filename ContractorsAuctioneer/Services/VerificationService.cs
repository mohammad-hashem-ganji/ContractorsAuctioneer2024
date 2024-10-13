using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace ContractorsAuctioneer.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly Random _random = new Random();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManger;
        private readonly IAuthService _autService;

        public VerificationService(ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManger,
            IAuthService autService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManger = signInManger;
            _autService = autService;
        }



        public async Task<Result<GetVerificationCodeDto>> GenerateAndSendCodeAsync(int userId, string phoneNumber, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                const string key = "ParsianContractorAuthenearproject";
                string userName = user.UserName;
                if (userName.EndsWith(key))
                {
                    userName = userName.Substring(0, userName.Length - key.Length);
                }
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                await _userManager.UpdateAsync(user);
                var result = new GetVerificationCodeDto
                {
                    VerificationCodeCode = token,
                    PhoneNumber = user.PhoneNumber,
                    Ncode = userName
                };
                //await SendSmsAsync(phoneNumber, $"کد: {token}", cancellationToken);
                return new Result<GetVerificationCodeDto>()
                    .WithValue(result)
                    .Success(SuccessMessages.CodeGeneratedAndSent);
            }
            return new Result<GetVerificationCodeDto>().WithValue(null).Failure("کد ساخته نشد");
        }
        public async Task<Result<string>> VerifyCodeAsync(GetVerificationCodeDto verificationCodeDto, CancellationToken cancellationToken)
        {
            
            var result = await _autService.AuthenticateAsync(verificationCodeDto.Ncode, verificationCodeDto.PhoneNumber);
            if (result != null)
            {
                var isTokenValid = await _userManager
                    .VerifyTwoFactorTokenAsync(result.Data, TokenOptions.DefaultPhoneProvider, verificationCodeDto.VerificationCodeCode);

                if (isTokenValid)
                {
                    await _signInManger.SignInAsync(result.Data, isPersistent: false);
                }
                var token = await _autService.GenerateJwtTokenAsync(result.Data);
                if (token is not null)
                {
                    return new Result<string>()
                   .WithValue(token)
                   .Success("کد تایید شد");
                }                
            }
            return new Result<string>()
            .WithValue(null)
            .Failure("کاربر یافت نشد");
        }
        private async Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken)
        {
            try
            {
                var sender = _configuration["Kavenegar:Sender"];
                var apiKey = _configuration["Kavenegar:ApiKey"];
                var api = new Kavenegar.KavenegarApi(apiKey);
                api.Send(sender, phoneNumber, message);
            }
            catch (Kavenegar.Exceptions.ApiException ex)
            {
                throw new InvalidOperationException($"Kavenegar API error: {ex.Message}");
            }
            catch (Kavenegar.Exceptions.HttpException ex)
            {
                throw new InvalidOperationException($"Kavenegar HTTP error: {ex.Message}");
            }
        }

    }
}
