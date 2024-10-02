using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRequestStatusService
    {
        Task<Result<RequestStatusDto>> AddAsync(RequestStatusDto requestStatusDto, CancellationToken cancellationToken);
        Task<Result<RequestStatusDto>> GetByIdAsync(int reqStatusId, CancellationToken cancellationToken);
        Task<Result<RequestStatusDto>> UpdateAsync(RequestStatusDto requestStatusDto, CancellationToken cancellationToken);
        Task<Result<RequestStatusDto>> GetRequestStatusByRequestId(int requesId, CancellationToken cancellationToken);

    }
}
