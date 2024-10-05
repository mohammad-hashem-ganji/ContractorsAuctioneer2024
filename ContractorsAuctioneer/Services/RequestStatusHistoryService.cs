using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;

namespace ContractorsAuctioneer.Services
{
    public class RequestStatusHistoryService : IRequestStatusHistoryService
    {
        private readonly ApplicationDbContext _context;

        public RequestStatusHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<AddRequestStatusHistoryDto>> AddAsync(AddRequestStatusHistoryDto requestStatusHistoryDto, CancellationToken cancellationToken)
        {
            if (requestStatusHistoryDto is null)
            {
                return new Result<AddRequestStatusHistoryDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
                var requestStatusHistory = new RequestStatusHistory
                {
                    RequestStatusId = requestStatusHistoryDto.RequetStatusId,
                    Status = requestStatusHistoryDto.Status,
                    CreatedAt = requestStatusHistoryDto.CreatedAt,
                    CreatedBy = requestStatusHistoryDto.CreatedBy
                };
                await _context.RequestStatusHistories.AddAsync(requestStatusHistory, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<AddRequestStatusHistoryDto>().WithValue(requestStatusHistoryDto).Success(SuccessMessages.RequestStatusHistoryAdded);
            }
            catch (Exception)
            {
                return new Result<AddRequestStatusHistoryDto>().WithValue(null).Failure(ErrorMessages.ErrorWhileAddingRequestStatuHistory);
            }
        }
    }
}
