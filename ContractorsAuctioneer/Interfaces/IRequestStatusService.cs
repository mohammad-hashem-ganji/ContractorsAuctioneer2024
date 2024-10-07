﻿using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRequestStatusService
    {
        Task<Result<AddRequestStatusDto>> AddAsync(AddRequestStatusDto requestStatusDto, CancellationToken cancellationToken);
        Task<Result<RequestStatusDto>> GetByIdAsync(int reqStatusId, CancellationToken cancellationToken);
        Task<Result<RequestStatusDto>> UpdateAsync(RequestStatusDto requestStatusDto, CancellationToken cancellationToken);
        Task<Result<List<RequestStatusDto>>> GetRequestStatusesByRequestId(int requesId, CancellationToken cancellationToken);

    }
}
