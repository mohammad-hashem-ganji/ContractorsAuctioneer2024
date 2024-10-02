using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRejectedRequestService
    {
        Task<Result<UpdateRejectedRequestDto>> AddAsync(UpdateRejectedRequestDto requestDto, CancellationToken cancellationToken);
    }
}
