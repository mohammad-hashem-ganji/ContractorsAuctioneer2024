using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRequestService
    {
        Task<bool> AddAsync(AddRequestDto requestDto, CancellationToken cancellationToken);
        Task<Result<List<RequestDto>>?> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<RequestDto>> GetByIdAsync(int reqId, CancellationToken cancellationToken);
    }
}
