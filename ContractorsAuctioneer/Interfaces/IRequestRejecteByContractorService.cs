using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRequestRejecteByContractorService
    {
        Task<Result<AddRejectedRequestDto>> AddAsync(AddRejectedRequestDto rejectedRequestDto, CancellationToken cancellationToken);
    }
}
