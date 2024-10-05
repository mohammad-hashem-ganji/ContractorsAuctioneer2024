using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IVerificationService
    {
       
        Task<Result<string>> GenerateAndSendCodeAsync(int userId, string phoneNumber, CancellationToken cancellationToken);
        Task<Result<GetVerificationCodeDto>> VerifyCodeAsync(GetVerificationCodeDto verificationCodeDto, CancellationToken cancellationToken);

    }
}
