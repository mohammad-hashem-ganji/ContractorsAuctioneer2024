using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;

namespace ContractorsAuctioneer.Services
{
    public class RequestRejecteByContractorService : IRequestRejecteByContractorService
    {
        private readonly ApplicationDbContext _context;

        public RequestRejecteByContractorService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<AddRejectedRequestDto>> AddAsync(AddRejectedRequestDto rejectedRequestDto, CancellationToken cancellationToken)
        {
            if (rejectedRequestDto is null)
            {
                return new Result<AddRejectedRequestDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
                var newRejectedRequest = new RequestRejectedByContractor
                {
                    ContractorId = rejectedRequestDto.ContractorId,
                    RequestId = rejectedRequestDto.RequestId,
                    CreatedAt = rejectedRequestDto.CreatedAt,
                    CreatedBy = rejectedRequestDto.CreatedBy
                };
                await _context.RequestRejectedByContractors.AddAsync(newRejectedRequest, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<AddRejectedRequestDto>().WithValue(rejectedRequestDto).Success(SuccessMessages.RequestSuccessfullyRejectedByContracotr);
            }
            catch (Exception)
            {

                return new Result<AddRejectedRequestDto>().WithValue(null).Failure(ErrorMessages.ErrorWhileRejectingRequest);
            }
        }
    }
}
