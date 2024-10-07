using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRejectedRequestByClientService
    {
        Task<Result<UpdateRequestAcceptanceDto>> AddAsync(UpdateRequestAcceptanceDto requestDto, CancellationToken cancellationToken);
    }
}
