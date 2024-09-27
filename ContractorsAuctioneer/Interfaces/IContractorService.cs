﻿using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IContractorService
    {
        Task<Result<AddContractorDto>> AddAsync(AddContractorDto contractorDto, CancellationToken cancellationToken);
        Task<Result<ContractorDto>> GetByIdAsync(int contractorId, CancellationToken cancellationToken);
    }
}
