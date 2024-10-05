using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRequestStatusHistoryService
    {
        Task<Result<AddRequestStatusHistoryDto>> AddAsync(AddRequestStatusHistoryDto requestStatusHistoryDto, CancellationToken cancellationToken);
    }
}
