using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;

namespace ContractorsAuctioneer.Services
{
    public class RejectedRequestService : IRejectedRequestService
    {
        private readonly ApplicationDbContext _context;

        public RejectedRequestService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<UpdateRejectedRequestDto>> AddAsync(UpdateRejectedRequestDto requestDto, CancellationToken cancellationToken)
        {
            if (requestDto == null)
            {
                return new Result<UpdateRejectedRequestDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
                var rejectedRequest = new RejectedRequest
                {
                    Comment = requestDto.Comment,
                    RequestId = requestDto.RequestId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = requestDto.CreatedBy,
                };
                await _context.RejectedRequests.AddAsync(rejectedRequest, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                //از اینجا به بعد باید به سامانه مهندسی ارجا داده بشه.
                return new Result<UpdateRejectedRequestDto>().WithValue(requestDto).Success(SuccessMessages.RejectedrequestAdded);
            }
            catch (Exception ex)
            {
                return new Result<UpdateRejectedRequestDto>().WithValue(null).Failure(ex.Message);
            }
        }
    }
}
