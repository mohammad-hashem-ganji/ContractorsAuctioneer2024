using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly Random _random = new Random();

        public VerificationService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result<GetVerificationCodeDto>> GetLatestCode(int applicationUserId, CancellationToken cancellationToken)
        {
            try
            {
                var verificationCode = await _context.VerificationCodes
                           .Where(vc => vc.ApplicationUserId == applicationUserId)
                           .OrderByDescending(vc => vc.ExpiredTime)
                           .FirstOrDefaultAsync(cancellationToken);
                if (verificationCode == null)
                {
                    return new Result<GetVerificationCodeDto>()
                        .WithValue(null)
                        .Failure(ErrorMessages.VerificationCodeNotFound);
                }
                var verificationCodeDto = new GetVerificationCodeDto
                {
                    Code = verificationCode.Code,
                    ExpiredTime = verificationCode.ExpiredTime,
                    ApplicationUserId = verificationCode.ApplicationUserId
                };
                return new Result<GetVerificationCodeDto>()
                    .WithValue(verificationCodeDto)
                    .Success(SuccessMessages.VerificationCodeFound);
            }
            catch (Exception ex)
            {
                return new Result<GetVerificationCodeDto>()
                    .WithValue(null)
                    .Failure(ex.Message);
            }
        }

        public async Task<Result<string>> GenerateAndSendCodeAsync(int userId, string phoneNumber, CancellationToken cancellationToken)
        {
            var code = _random.Next(10000, 99999).ToString();
            var verificationCode = new AddVerificationCodeDto
            {
                Code = code,
                ExpiredTime = DateTime.Now.AddMinutes(5),
                ApplicationUserId = userId
            };
            await SaveVerificationCodeToDatabase(verificationCode, cancellationToken);
            await SendSmsAsync(phoneNumber, $"Your verification code is {code}");
            return new Result<string>()
                .WithValue(code)
                .Success(SuccessMessages.CodeGeneratedAndSent);
        }
        // generate it as result ###
        private async Task<bool> SaveVerificationCodeToDatabase(AddVerificationCodeDto verificationCode, CancellationToken cancellationToken)
        {
            var newVerificationCode = new VerificationCode
            {
                ApplicationUserId = verificationCode.ApplicationUserId,
                Code = verificationCode.Code,
                ExpiredTime = verificationCode.ExpiredTime,
                CreatedAt = DateTime.Now
            };
            await _context.VerificationCodes.AddAsync(newVerificationCode, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        // generate it as result ###
        private async Task SendSmsAsync(string phoneNumber, string message)
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
