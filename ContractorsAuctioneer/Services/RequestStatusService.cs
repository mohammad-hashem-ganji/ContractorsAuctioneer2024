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
        public async Task<Result<AddRequestStatusDto>> AddAsync(AddRequestStatusDto requestStatusDto, CancellationToken cancellationToken) 
        {
            if (requestStatusDto is null)
            {
                return new Result<AddRequestStatusDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
                var requestStatus = new RequestStatus
                {
                    RequestId = requestStatusDto.RequestId,
                    Status = requestStatusDto.Status,
                    CreatedBy = requestStatusDto.CreatedBy,
                    CreatedAt = requestStatusDto.CreatedAt,
                };
                await _context.RequestStatuses.AddAsync(requestStatus, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new Result<AddRequestStatusDto>().WithValue(requestStatusDto).Success(SuccessMessages.RequestStatusAdded);
            }
            catch (Exception)
            {
                return new Result<AddRequestStatusDto>().WithValue(null).Failure(ErrorMessages.ErrorWhileAddingRequestStatus);         
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
                    };
                    return new Result<RequestStatusDto>()
                        .WithValue(requestStatusDto)
                        .Success("وضعیت پیدا شد .");
                }   
            }
            catch (Exception)
            {
                return new Result<RequestStatusDto>()
                    .WithValue(null)
                    .Failure(ErrorMessages.ErrorWhileRetrievingStatus);
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
                requestStatus.UpdatedAt = DateTime.Now;
                //requestStatus.UpdatedBy = requestStatusDto.UpdatedBy;
                _context.RequestStatuses.Update(requestStatus);
                await _context.SaveChangesAsync();

                return new Result<RequestStatusDto>()
                    .WithValue(requestStatusDto)
                    .Success($"وضعیت تغییر به {requestStatusDto.Status}تغییر پیدا کرد.");
            }
            catch (Exception )
            {
                return new Result<RequestStatusDto>()
                    .WithValue(null).Failure(ErrorMessages.AnErrorWhileUpdatingStatus);
            }
        }
        public async Task<Result<List<RequestStatusDto>>> GetRequestStatusesByRequestId(int requesId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _context.RequestStatuses
                    .Where(x =>
                    x.RequestId == requesId )
                    .Include(s => s.Request)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(r => new RequestStatusDto
                    {
                        RequestId = r.RequestId,
                        Status = r.Status,
                    }).ToListAsync(cancellationToken);
                if (result is null) return new Result<List<RequestStatusDto>>()
                        .WithValue(null)
                        .Failure(ErrorMessages.RequestStatusNotFound);
                else return new Result<List<RequestStatusDto>>()
                        .WithValue(result)
                        .Success(SuccessMessages.RequestStatusFound);
            }
            catch (Exception)
            {
                return new Result<List<RequestStatusDto>>()
                    .WithValue(null)
                    .Failure(ErrorMessages.ErroWhileRetrievingRequestStatus);
            }
        }
    }
}



