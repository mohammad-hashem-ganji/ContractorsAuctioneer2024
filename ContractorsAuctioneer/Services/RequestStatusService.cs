using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Threading;

namespace ContractorsAuctioneer.Services
{
    public class RequestStatusService : IRequestStatusService
    {
        private readonly ApplicationDbContext _context;

        public RequestStatusService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<RequestStatusDto>> AddAsync(RequestStatusDto requestStatusDto, CancellationToken cancellationToken)
        {
            if (requestStatusDto is null)
            {
                return new Result<RequestStatusDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
                var requestStatus = new RequestStatus
                {
                    RequestId = requestStatusDto.Id,
                    Status = requestStatusDto.Status,
                    CreatedBy = requestStatusDto.CreatedBy,
                    CreatedAt = requestStatusDto.CreatedAt,
                };
                await _context.RequestStatuses.AddAsync(requestStatus, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<RequestStatusDto>().WithValue(requestStatusDto).Success(SuccessMessages.RequestStatusAdded);
            }
            catch (Exception ex)
            {
                return new Result<RequestStatusDto>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<RequestStatusDto>> GetByIdAsync(int reqStatusId, CancellationToken cancellationToken)
        {
            try
            {
                RequestStatus? requestStatus = await _context.RequestStatuses
                    .Where(x => x.Id == reqStatusId)
                    .Include(s => s.Request)
                    .FirstOrDefaultAsync(cancellationToken);
                if (requestStatus == null)
                {
                    return new Result<RequestStatusDto>()
                        .WithValue(null)
                        .Failure(ErrorMessages.EntityIsNull);
                }
                else
                {
                    var requestStatusDto = new RequestStatusDto
                    {
                        Id = requestStatus.Id,
                        Status = requestStatus.Status,
                        RequestId = requestStatus.RequestId,
                        UpdatedBy = requestStatus.UpdatedBy,
                        UpdatedAt = requestStatus.UpdatedAt
                    };
                    return new Result<RequestStatusDto>()
                        .WithValue(requestStatusDto)
                        .Success("وضعیت تغییر یافت.");
                }   
            }
            catch (Exception ex)
            {
                return new Result<RequestStatusDto>()
                    .WithValue(null)
                    .Failure(ex.Message);
            }
        }
        public async Task<Result<RequestStatusDto>> UpdateAsync(RequestStatusDto requestStatusDto, CancellationToken cancellationToken)
        {
            try
            {
                RequestStatus? requestStatus = await _context.RequestStatuses
                    .Where(x => x.Id == requestStatusDto.Id)
                    .Include(s => s.Request)
                    .FirstOrDefaultAsync(cancellationToken);
                if (requestStatus == null)
                {
                    return new Result<RequestStatusDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
                }
                requestStatus.RequestId = requestStatusDto.RequestId;
                requestStatus.Status = requestStatusDto.Status;
                requestStatus.UpdatedAt = requestStatusDto.UpdatedAt;
                requestStatus.UpdatedBy = requestStatusDto.UpdatedBy;
                return new Result<RequestStatusDto>()
                    .WithValue(requestStatusDto)
                    .Success($"وضعیت تغییر به {requestStatusDto.Status}تغییر پیدا کرد.");
            }
            catch (Exception ex)
            {
                return new Result<RequestStatusDto>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<RequestStatusDto>> GetRequestStatusByRequestId(int requesId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _context.RequestStatuses
                    .Where(x =>
                    x.RequestId == requesId && x.IsDeleted == false)
                    .Include(s => s.Request)
                    .Include(s => s.RequestStatusHistories)
                    .Select(r => new RequestStatusDto
                    {
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        UpdatedBy = r.UpdatedBy,
                        RequestId = r.RequestId,
                        Status = r.Status,
                    }).FirstOrDefaultAsync(cancellationToken);
                if (result is null) return new Result<RequestStatusDto>()
                        .WithValue(null)
                        .Failure(ErrorMessages.RequestStatusNotFound);
                else return new Result<RequestStatusDto>()
                        .WithValue(result)
                        .Success(SuccessMessages.RequestStatusFound);
            }
            catch (Exception)
            {
                return new Result<RequestStatusDto>()
                    .WithValue(null)
                    .Failure(ErrorMessages.ErroWhileRetrievingRequestStatus);
            }
        }
    }
}
