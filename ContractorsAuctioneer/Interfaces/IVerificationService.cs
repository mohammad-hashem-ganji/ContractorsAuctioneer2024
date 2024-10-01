using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IVerificationService
    {
        Task<Result<GetVerificationCodeDto>> GetLatestCode(int applicationUserId, CancellationToken cancellationToken);
        Task<Result<string>> GenerateAndSendCodeAsync(int userId, string phoneNumber, CancellationToken cancellationToken);
        //Task<bool> SaveVerificationCodeToDatabase(AddVerificationCodeDto verificationCode, CancellationToken cancellationToken);
        //Task SendSmsAsync(string phoneNumber, string message);

    }
}
