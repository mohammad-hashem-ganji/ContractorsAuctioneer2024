using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IBidOfContractorService
    {
        Task<Result<AddBidOfContractorDto>> AddAsync(AddBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken);
        Task<Result<BidOfContractorDto>> GetByIdAsync(int bidId, CancellationToken cancellationToken);
        Task<Result<List<BidOfContractorDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<BidOfContractorDto>> UpdateAsync(BidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken);
    }
}
