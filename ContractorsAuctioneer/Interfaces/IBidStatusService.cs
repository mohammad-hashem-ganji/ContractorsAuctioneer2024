using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IBidStatusService
    {
        Task<Result<AddBidStatusDto>> AddAsync(AddBidStatusDto bidDto, CancellationToken cancellationToken);
        Task<Result<List<BidStatusDto>>> GetRequestStatusesByBidId(int bidId, CancellationToken cancellationToken);
    }
}
