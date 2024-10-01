using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface ILastLoginHistoryService
    {
        Task<Result<AddLastLoginHistoryDto>> AddAsync(AddLastLoginHistoryDto lastLoginHistoryDto, CancellationToken cancellationToken);
        Task<Result<UpdateLastLoginHistoryDto>> UpdateAsync(UpdateLastLoginHistoryDto lastLoginHistoryDto, CancellationToken cancellationToken);
    }
}
