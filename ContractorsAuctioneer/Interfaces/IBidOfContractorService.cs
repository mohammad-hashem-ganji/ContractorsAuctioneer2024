﻿using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IBidOfContractorService
    {
        Task<Result<AddBidOfContractorDto>> AddAsync(AddBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken);
        Task<Result<BidOfContractorDto>> GetByIdAsync(int bidId, CancellationToken cancellationToken);
        Task<Result<List<BidOfContractorDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<UpdateBidOfContractorDto>> UpdateAsync(UpdateBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken);
        Task<Result<List<BidOfContractorDto>>> GetBidsOfContractorAsync(CancellationToken cancellationToken);
        Task<Result<List<BidOfContractorDto>>> GetBidsOfRequestAsync(int requestId, CancellationToken cancellationToken);
        Task<Result<List<BidOfContractorDto>>> GetBidsAcceptedByClientAsync(CancellationToken cancellationToken);
        Task<Result<BidOfContractorDto>> CheckBidIsAcceptedByClientAsync(int bidId, CancellationToken cancellationToken);
    }
}
