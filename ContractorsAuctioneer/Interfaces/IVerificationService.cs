using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IVerificationService
    {
       
        Task<Result<GetVerificationCodeDto>> GenerateAndSendCodeAsync(int userId, string phoneNumber, CancellationToken cancellationToken);
        Task<Result<UserWithRoleAndTokenDto>> VerifyCodeAsync(GetVerificationCodeDto verificationCodeDto, CancellationToken cancellationToken);

    }
}
