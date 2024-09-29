using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class VerificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new Random();

        public VerificationService(ApplicationDbContext context)
        {
            _context = context;
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
                    return new Result<GetVerificationCodeDto>().WithValue(null).Failure(ErrorMessages.VerificationCodeNotFound);
                }
                var verificationCodeDto = new GetVerificationCodeDto
                {
                    Code = verificationCode.Code,
                    ExpiredTime = verificationCode.ExpiredTime,
                    ApplicationUserId = verificationCode.ApplicationUserId
                };
                return new Result<GetVerificationCodeDto>().WithValue(verificationCodeDto).Success(SuccessMessages.VerificationCodeFound);
            }
            catch (Exception ex)
            {
                return new Result<GetVerificationCodeDto>().WithValue(null).Failure(ex.Message);
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
            return new Result<string>().WithValue(code).Success(SuccessMessages.CodeGeneratedAndSent);
        }

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

        private async Task SendSmsAsync(string phoneNumber, string message)
        {
            using (var client = new HttpClient())
            {

                var content = new StringContent($"{{\"to\":\"{phoneNumber}\",\"message\":\"{message}\"}}"
                    , System.Text.Encoding.UTF8, "application/json");
                await client.PostAsync("https://your-sms-api.com/send", content);
            }
        }
    }
}
