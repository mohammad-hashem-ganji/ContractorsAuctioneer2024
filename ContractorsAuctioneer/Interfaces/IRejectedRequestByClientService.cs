using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRejectedRequestByClientService
    {
        Task<Result<UpdateRejectedRequestDto>> AddAsync(UpdateRejectedRequestDto requestDto, CancellationToken cancellationToken);
    }
}
