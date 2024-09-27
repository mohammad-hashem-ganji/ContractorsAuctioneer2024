using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using Azure.Core;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ContractorsAuctioneer.Services
{
    public class BidOfContractorService
    {
        //Add bid
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
                return new Result<AddBidOfContractorDto>().WithValue(null).Failure(ex.Message);
            }
            // update bid
            // delete bid 
            // get all bids
            // get bid by Id
            // get bids by contractorId => I mean showing contractor bids
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
                            CreatedAt = b.CreatedAt,
                            CreatedBy = b.CreatedBy,
                            DeletedAt = b.DeletedAt,
                            DeletedBy = b.DeletedBy,
                            IsDeleted = b.IsDeleted,
                            UpdatedAt = b.UpdatedAt,
                            UpdatedBy = b.UpdatedBy
                        }).ToList()
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
                        CreatedBy = b.CreatedBy,
                        DeletedAt = b.DeletedAt,
                        DeletedBy = b.DeletedBy,
                        IsDeleted = b.IsDeleted,
                        UpdatedAt = b.UpdatedAt,
                        UpdatedBy = b.UpdatedBy
                    }).ToList()

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
                return new Result<List<BidOfContractorDto>>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<BidOfContractorDto>> UpdateAsync(BidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken)
        {
            try
            {

                BidOfContractor? bidOfContractor = await _context.BidOfContractors
                  .Where(x => x.Id == bidOfContractorDto.Id)
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
    }
}