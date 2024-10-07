using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using Azure.Core;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace ContractorsAuctioneer.Services
{
    public class BidOfContractorService : IBidOfContractorService
    {

        private readonly ApplicationDbContext _context;
        private readonly IContractorService _contractorService;
        public BidOfContractorService(ApplicationDbContext context, IContractorService contractorService)
        {
            _context = context;
            _contractorService = contractorService;
        }
        public async Task<Result<AddBidOfContractorDto>> AddAsync(AddBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken)
        {
            if (bidOfContractorDto == null)
            {
                return new Result<AddBidOfContractorDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
                var bidOfContractor = new BidOfContractor
                {
                    SuggestedFee = bidOfContractorDto.SuggestedFee,
                    CanChangeBid = true,
                    ContractorId = bidOfContractorDto.ContractorId,
                    RequestId = bidOfContractorDto.RequestId,
                    IsAccepted = false,
                    CreatedAt = DateTime.Now
                };
                await _context.BidOfContractors.AddAsync(bidOfContractor, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<AddBidOfContractorDto>().WithValue(bidOfContractorDto).Success(SuccessMessages.OperationSuccessful);
            }
            catch (Exception ex)
            {
                return new Result<AddBidOfContractorDto>().WithValue(null).Failure(ErrorMessages.ErrorWileAddingBidOfContractor);
            }
        }
        public async Task<Result<BidOfContractorDto>> GetByIdAsync(int bidId, CancellationToken cancellationToken)
        {
            try
            {
                BidOfContractor? bidOfContractor = await _context.BidOfContractors
                    .Where(x => x.Id == bidId)
                    .Include(x => x.Contractor)
                    .Include(x => x.Request)
                    .Include(x => x.BidStatuses)
                    .FirstOrDefaultAsync(cancellationToken);
                if (bidOfContractor == null)
                {
                    return new Result<BidOfContractorDto>().WithValue(null).Failure(ErrorMessages.BidOfContractorNotFound);
                }
                else
                {
                    if (bidOfContractor.IsDeleted == true)
                    {
                        return new Result<BidOfContractorDto>().WithValue(null).Failure(ErrorMessages.BidIsDeleted);
                    }
                    var bidOfContractorDto = new BidOfContractorDto
                    {
                        Id = bidOfContractor.Id,
                        CanChangeBid = bidOfContractor.CanChangeBid,
                        RequestId = bidOfContractor.RequestId,
                        CreatedAt = bidOfContractor.CreatedAt,
                        CreatedBy = bidOfContractor.CreatedBy,
                        ContractorId = bidOfContractor.ContractorId,
                        IsAccepted = bidOfContractor.IsAccepted,
                        IsDeleted = bidOfContractor.IsDeleted,
                        UpdatedBy = bidOfContractor.UpdatedBy,
                        UpdatedAt = bidOfContractor.UpdatedAt,
                        SuggestedFee = bidOfContractor.SuggestedFee,
                        BidStatuses = bidOfContractor.BidStatuses.Select(b => new BidStatus
                        {
                            Id = b.Id,
                            Status = b.Status,
                            UpdatedAt = b.UpdatedAt,
                            UpdatedBy = b.UpdatedBy
                        })
                        .OrderByDescending(b => b.CreatedAt)
                        .ToList()
                    };
                    return new Result<BidOfContractorDto>().WithValue(bidOfContractorDto).Success("پیشنهاد پیدا شد");
                }
            }
            catch (Exception ex)
            {
                return new Result<BidOfContractorDto>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<List<BidOfContractorDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var bidsOfContractor = await _context.BidOfContractors
                    .Include(x => x.Contractor)
                    .Include(x => x.Request)
                    .Include(x => x.BidStatuses)
                    .ToListAsync(cancellationToken);
                var bidsOfContractorDto = bidsOfContractor.Where(a => a.IsDeleted == false).Select(x => new BidOfContractorDto
                {
                    Id = x.Id,
                    CanChangeBid = x.CanChangeBid,
                    RequestId = x.RequestId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    ContractorId = x.ContractorId,
                    IsAccepted = x.IsAccepted,
                    IsDeleted = x.IsDeleted,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedAt = x.UpdatedAt,
                    SuggestedFee = x.SuggestedFee,
                    BidStatuses = x.BidStatuses.Select(b => new BidStatus
                    {
                        Id = b.Id,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt,
                        CreatedBy = b.CreatedBy
                    })
                    .OrderByDescending(b => b.CreatedAt)
                    .ToList()

                }).ToList();
                if (bidsOfContractorDto.Any())
                {
                    return new Result<List<BidOfContractorDto>>().WithValue(bidsOfContractorDto).Success("پیشنهاد ها یافت شدند .");
                }
                else
                {
                    return new Result<List<BidOfContractorDto>>().WithValue(bidsOfContractorDto).Failure("پیشنهادی وجود ندارد");
                }
            }
            catch (Exception ex)
            {
                return new Result<List<BidOfContractorDto>>().WithValue(null).Failure(ErrorMessages.ErrorWhileRetrievingBidsOfContracotrs);
            }
        }
        public async Task<Result<BidOfContractorDto>> UpdateAsync(BidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken)
        {
            try
            {

                BidOfContractor? bidOfContractor = await _context.BidOfContractors
                  .Where(x => x.Id == bidOfContractorDto.Id && x.IsDeleted == false)
                  .FirstOrDefaultAsync(cancellationToken);
                if (bidOfContractor is null)
                {
                    return new Result<BidOfContractorDto>().WithValue(null).Failure(ErrorMessages.BidOfContractorNotFound);
                }
                bidOfContractor.UpdatedAt = bidOfContractorDto.UpdatedAt;
                bidOfContractor.UpdatedBy = bidOfContractorDto.UpdatedBy;
                bidOfContractor.IsAccepted = bidOfContractorDto.IsAccepted;
                bidOfContractor.CanChangeBid = bidOfContractorDto.CanChangeBid;
                bidOfContractor.IsDeleted = bidOfContractorDto.IsDeleted;
                bidOfContractor.SuggestedFee = bidOfContractorDto.SuggestedFee;
                _context.BidOfContractors.Update(bidOfContractor);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<BidOfContractorDto>().WithValue(bidOfContractorDto).Success("پیشنهاد آپدیت شد");
            }
            catch (Exception ex)
            {
                return new Result<BidOfContractorDto>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<List<BidOfContractorDto>>> GetBidsOfContractorAsync(int contractorId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _context
                .BidOfContractors
                .Where(x => x.ContractorId == contractorId && x.IsDeleted == false)
                .Select(x => new BidOfContractorDto
                {
                    ContractorId = x.ContractorId,
                    Id = x.Id,
                    CanChangeBid = x.CanChangeBid,
                    IsAccepted = x.IsAccepted,
                    SuggestedFee = x.SuggestedFee,
                    IsDeleted = x.IsDeleted,
                    RequestId = x.RequestId,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedAt = x.UpdatedAt,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    BidStatuses = x.BidStatuses.Select(b => new BidStatus
                    {
                        Id = b.Id,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt,
                        CreatedBy = b.CreatedBy,
                        DeletedAt = b.DeletedAt,
                        DeletedBy = b.DeletedBy,
                        IsDeleted = b.IsDeleted,
                        UpdatedAt = b.UpdatedAt,
                        UpdatedBy = b.UpdatedBy
                    }).ToList()
                }).ToListAsync(cancellationToken);
                if (result.Any())
                {
                    return new Result<List<BidOfContractorDto>>().WithValue(result).Success("پیشنهاد ها یافت شدند .");
                }
                else
                {
                    return new Result<List<BidOfContractorDto>>().WithValue(null).Failure("پیشنهادی وجود ندارد");
                }
            }
            catch (Exception)
            {
                return new Result<List<BidOfContractorDto>>().WithValue(null).Failure("خطایی در بازیابی پیشنهادات رخ داده است.");
            }

        }
        public async Task<Result<List<BidOfContractorDto>>> GetBidsOfRequestAsync(int requestId, CancellationToken cancellationToken)
        {
            try
            {
                List<BidOfContractorDto> bidsOfContractor = await _context.BidOfContractors
                        .Where(x => x.RequestId == requestId
                        && x.Request.IsTenderOver == true
                        && x.Request.IsActive == true
                        && x.IsDeleted == false)
                        .Include(x => x.Request)
                        .Select(bid => new BidOfContractorDto
                        {
                            Id = bid.Id,
                            RequestId = bid.RequestId,
                            CreatedAt = bid.CreatedAt,
                            ContractorId = bid.ContractorId,
                            IsAccepted = bid.IsAccepted,
                            SuggestedFee = bid.SuggestedFee,
                        })
                        .ToListAsync(cancellationToken);
                if (bidsOfContractor.Any())
                {
                    return new Result<List<BidOfContractorDto>>()
                        .WithValue(bidsOfContractor)
                        .Success(SuccessMessages.BidsOfRequestFound);
                }
                else
                {
                    return new Result<List<BidOfContractorDto>>()
                        .WithValue(null)
                        .Failure(ErrorMessages.BidsOfRequestNotFound);
                }
            }
            catch (Exception)
            {
                return new Result<List<BidOfContractorDto>>()
                    .WithValue(null)
                    .Failure(ErrorMessages.ErrorWhileRetrievingBidsOfContracotrs);
            }

        }
        public async Task<Result<List<BidOfContractorDto>>> UnAcceptRestBidsOfRequestAsync(int requestId,
             List<int> unAcceptedBidsId,CancellationToken cancellationToken)
        {
            try
            {
            List<BidOfContractorDto> bidsOfContractor = await _context.BidOfContractors
                    .Where(x => x.RequestId == requestId
                    && unAcceptedBidsId.Contains(x.Id)
                    && x.Request.IsTenderOver == true
                    && x.Request.IsActive == true
                    && x.IsDeleted == false)
                    .Include(x => x.Request)
                    .Select(bid => new BidOfContractorDto
                    {
                        Id = bid.Id,
                        RequestId = bid.RequestId,
                        CreatedAt = bid.CreatedAt,
                        ContractorId = bid.ContractorId,
                        IsAccepted = bid.IsAccepted,
                        SuggestedFee = bid.SuggestedFee,
                    })
                    .ToListAsync(cancellationToken);


                if (bidsOfContractor.Any())
                {
                    bidsOfContractor.ForEach(x => x.IsAccepted = false);
                    //_context.UpdateRange(bidsOfContractor);
                    await _context.SaveChangesAsync(cancellationToken);
                    return new Result<List<BidOfContractorDto>>()
                        .WithValue(bidsOfContractor)
                        .Success(SuccessMessages.BidsOfRequestFoundAndChangedAcceptance);
                }
                else
                {
                    return new Result<List<BidOfContractorDto>>()
                        .WithValue(null)
                        .Failure(ErrorMessages.BidsOfRequestNotFound);
                }
            }
            catch (Exception)
            {
                return new Result<List<BidOfContractorDto>>()
                        .WithValue(null)
                        .Failure(ErrorMessages.ErrorWhileRetrievingBidsOfContracotrs);
            }
        }

    }
}