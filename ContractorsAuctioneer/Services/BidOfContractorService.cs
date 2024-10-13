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
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;

namespace ContractorsAuctioneer.Services
{
    public class BidOfContractorService : IBidOfContractorService
    {

        private readonly ApplicationDbContext _context;
        private readonly IContractorService _contractorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BidOfContractorService(ApplicationDbContext context,
            IContractorService contractorService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contractorService = contractorService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<AddBidOfContractorDto>> AddAsync(AddBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken)
        {
            
            if (bidOfContractorDto == null)
            {
                return new Result<AddBidOfContractorDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
            
                var result = await UserManagement.GetRoleBaseUserId(_httpContextAccessor.HttpContext, _context);
                if (!result.IsSuccessful)
                {
                    var errorMessage = result.Message ?? "خطا !";
                    return new Result<AddBidOfContractorDto>().WithValue(null).Failure(errorMessage);
                }

                var user = result.Data;
                if (user is null)
                {
                    return new Result<AddBidOfContractorDto>().WithValue(null).Failure("خطا.");
                }

                var bidOfContractor = new BidOfContractor
                {
                    SuggestedFee = bidOfContractorDto.SuggestedFee,
                    CanChangeBid = true,
                    ContractorId = user.UserId,
                    RequestId = bidOfContractorDto.RequestId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = user.UserId
                };
                await _context.BidOfContractors.AddAsync(bidOfContractor, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<AddBidOfContractorDto>().WithValue(bidOfContractorDto).Success(SuccessMessages.OperationSuccessful);
            }
            catch (Exception)
            {
                return new Result<AddBidOfContractorDto>().WithValue(null).Failure(ErrorMessages.ErrorWileAddingBidOfContractor);
            }
        }
        public async Task<Result<BidOfContractorDto>> GetByIdAsync(int bidId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await UserManagement.GetRoleBaseUserId(_httpContextAccessor.HttpContext, _context);
                if (!result.IsSuccessful)
                {
                    var errorMessage = result.Message ?? "خطا !";
                    return new Result<BidOfContractorDto>().WithValue(null).Failure(errorMessage);
                }

                var user = result.Data;

                BidOfContractor? bidOfContractor = await _context.BidOfContractors
                    .Where(x => x.Id == bidId && x.ContractorId == user.UserId)
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
                        RequestId = bidOfContractor.RequestId,
                        ContractorId = bidOfContractor.ContractorId,
                        SuggestedFee = bidOfContractor.SuggestedFee,
                        CreatedAt = bidOfContractor.CreatedAt,
                        BidStatuses = bidOfContractor.BidStatuses.Select(b => new BidStatus
                        {
                            Id = b.Id,
                            Status = b.Status,
                            UpdatedAt = b.UpdatedAt,
                            UpdatedBy = b.CreatedBy
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
                    RequestId = x.RequestId,
                    ContractorId = x.ContractorId,
                    SuggestedFee = x.SuggestedFee,
                    CreatedAt = x.CreatedAt,
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
        public async Task<Result<UpdateBidOfContractorDto>> UpdateAsync(UpdateBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken)
        {
            try
            {
               
          
                var result = await UserManagement.GetRoleBaseUserId(_httpContextAccessor.HttpContext, _context);
                if (!result.IsSuccessful)
                {
                    var errorMessage = result.Message ?? "خطا !";
                    return new Result<UpdateBidOfContractorDto>().WithValue(null).Failure(errorMessage);
                }

                var user = result.Data;
                BidOfContractor? bidOfContractor = await _context.BidOfContractors
                  .Where(x => x.Id == bidOfContractorDto.Id )
                  .FirstOrDefaultAsync(cancellationToken);
                if (bidOfContractor is null)
                {
                    return new Result<UpdateBidOfContractorDto>().WithValue(null).Failure(ErrorMessages.BidOfContractorNotFound);
                }
                bidOfContractor.UpdatedAt = DateTime.Now;
                bidOfContractor.UpdatedBy = user.UserId;
                bidOfContractor.CanChangeBid = bidOfContractorDto.CanChangeBid;
                bidOfContractor.IsDeleted = bidOfContractorDto.IsDeleted;
                bidOfContractor.SuggestedFee = bidOfContractorDto.SuggestedFee;
                _context.BidOfContractors.Update(bidOfContractor);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<UpdateBidOfContractorDto>().WithValue(bidOfContractorDto).Success("پیشنهاد آپدیت شد");
            }
            catch (Exception ex)
            {
                return new Result<UpdateBidOfContractorDto>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<List<BidOfContractorDto>>> GetBidsOfContractorAsync(CancellationToken cancellationToken)
        {
            try
            {
               
                var result = await UserManagement.GetRoleBaseUserId(_httpContextAccessor.HttpContext, _context);
                if (!result.IsSuccessful)
                {
                    var errorMessage = result.Message ?? "خطا !";
                    return new Result<List<BidOfContractorDto>>().WithValue(null).Failure(errorMessage);
                }

                var user = result.Data;
                var bids = await _context
                .BidOfContractors
                .Where(x => x.ContractorId == user.UserId && x.IsDeleted == false)
                .Select(x => new BidOfContractorDto
                {
                    ContractorId = x.ContractorId,
                    Id = x.Id,
                    SuggestedFee = x.SuggestedFee,
                    IsDeleted = x.IsDeleted,
                    RequestId = x.RequestId,
                    CreatedAt = x.CreatedAt,
                    BidStatuses = x.BidStatuses.Select(b => new BidStatus
                    {
                        Id = b.Id,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt,
                        CreatedBy = b.CreatedBy
                    }).ToList()
                }).ToListAsync(cancellationToken);
                if (bids.Any())
                {
                    return new Result<List<BidOfContractorDto>>().WithValue(bids).Success("پیشنهاد ها یافت شدند .");
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
                        .Where(x => (x.RequestId == requestId
                        && x.Request.IsTenderOver == true
                        && x.Request.IsActive == true
                        && x.IsDeleted == false)
                        || (x.BidStatuses.Any(b => b.Status == BidStatusEnum.BidRejectedByContractor  &&b.ContractorBidId == x.Id)
                        ))
                        .Include(x => x.Request)
                        .Select(bid => new BidOfContractorDto
                        {
                            Id = bid.Id,
                            RequestId = bid.RequestId,
                            ContractorId = bid.ContractorId,
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

        public async Task<Result<List<BidOfContractorDto>>> GetBidsAcceptedByClient(CancellationToken cancellationToken)
        {
            var result = await UserManagement.GetRoleBaseUserId(_httpContextAccessor.HttpContext, _context);
            if (!result.IsSuccessful)
            {
                var errorMessage = result.Message ?? "خطا !";
                return new Result<List<BidOfContractorDto>>().WithValue(null).Failure(errorMessage);
            }

            var user = result.Data;
            var acceptedBids = await _context.BidOfContractors
                .Where(b => b.ContractorId ==user.UserId &&
                b.BidStatuses.Any(x => x.Status == BidStatusEnum.BidApprovedByClient))
                .Include(x => x.BidStatuses)
                .Select(x => new BidOfContractorDto
                {
                    
                    Id = x.Id,
                    SuggestedFee = x.SuggestedFee,
                    RequestId = x.RequestId,

                }).ToListAsync(cancellationToken);
            if (acceptedBids.Count != 0)
            {
                return new Result<List<BidOfContractorDto>>()
                    .WithValue(acceptedBids)
                    .Success(SuccessMessages.AcceptedBidsFound);
            }
            else
            {
                return new Result<List<BidOfContractorDto>>()
                    .WithValue(null)
                    .Failure(ErrorMessages.BidOfContractorNotFound);
            }
        }



    }
}