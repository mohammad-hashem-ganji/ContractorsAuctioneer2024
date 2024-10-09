using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class BidStatusService : IBidStatusService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BidStatusService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<AddBidStatusDto>> AddAsync(AddBidStatusDto bidDto, CancellationToken cancellationToken)
        {
            if (bidDto is null)
            {
                return new Result<AddBidStatusDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext is null)
                {
                    return new Result<AddBidStatusDto>().WithValue(null).Failure("خطا.");
                }
                var result = await UserManagement.GetRoleBaseUserId(httpContext, _context);
                if (!result.IsSuccessful)
                {
                    var errorMessage = result.Message ?? "خطا !";
                    return new Result<AddBidStatusDto>().WithValue(null).Failure(errorMessage);
                }

                var user = result.Data;
                if (user is null)
                {
                    return new Result<AddBidStatusDto>().WithValue(null).Failure("خطا.");
                }
                var bidStatus = new BidStatus
                {
                    ContractorBidId = bidDto.BidOfContractorId,
                    CreatedBy = user.UserId,
                    Status = bidDto.Status,
                    CreatedAt = DateTime.Now
                };
                await _context.BidStatuses.AddAsync(bidStatus, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<AddBidStatusDto>().WithValue(bidDto).Success(SuccessMessages.BidStatusAdded);
            }
            catch (Exception)
            {
                return new Result<AddBidStatusDto>().WithValue(null).Failure(ErrorMessages.ErrorWhileAddingBidStatus);
            }
        }

        public async Task<Result<List<BidStatusDto>>> GetRequestStatusesByBidId(int bidId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _context.BidStatuses
                    .Where(x => x.ContractorBidId == bidId)
                    .Include(x => x.ContractorBid)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new BidStatusDto
                    {
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        BidOfContractorId = x.ContractorBidId,
                        Status = x.Status
                    }).ToListAsync(cancellationToken);
                if (result is null) return new Result<List<BidStatusDto>>()
                        .WithValue(null)
                        .Failure(ErrorMessages.BidStatusNotFound);
                else return new Result<List<BidStatusDto>>()
                        .WithValue(result)
                        .Success(SuccessMessages.BidStatusFound);
            }
            catch (Exception)
            {
                return new Result<List<BidStatusDto>>()
                   .WithValue(null)
                   .Failure(ErrorMessages.ErrorWhileRetrievingStatus);
            }
        }


    }
}
