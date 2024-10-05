using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ContractorsAuctioneer.Services
{
    public class ContractorService : IContractorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        
        public ContractorService(ApplicationDbContext context, IAuthService authService, IRequestRejecteByContractorService requestRejecteByContractorService)
        {
            _context = context;
            _authService = authService;
            _requestRejecteByContractorService = requestRejecteByContractorService;
        }
        public async Task<Result<AddContractorDto>> AddAsync(AddContractorDto contractorDto, CancellationToken cancellationToken)
        {
            try
            {
                const string role = "Constractor";
                var applicationUserResult = await _authService.RegisterAsync(contractorDto.Username, contractorDto.Password, role);
                if (applicationUserResult.Data.RegisteredUserId == 0)
                {
                    return new Result<AddContractorDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
                }
                var contractor = new Contractor
                {
                    Name = contractorDto.Name,
                    CompanyName = contractorDto.CompanyName,
                    Email = contractorDto.Email,
                    FaxNumber = contractorDto.FaxNumber,
                    Address = contractorDto.Address,
                    LandlineNumber = contractorDto.LandlineNumber,
                    MobileNumber = contractorDto.MobileNumber,
                    ApplicationUserId = applicationUserResult.Data.RegisteredUserId
                };
                await _context.Contractors.AddAsync(contractor, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<AddContractorDto>().WithValue(contractorDto).Success(SuccessMessages.ContractorAdded);
            }
            catch (Exception ex)
            {
                return new Result<AddContractorDto>().WithValue(null).Success(ex.Message);
            }

        }
        public async Task<Result<ContractorDto>> GetByIdAsync(int contractorId, CancellationToken cancellationToken)
        {
            try
            {
                var contractor = await _context.Contractors
                  .Where(x => x.Id == contractorId)
                  .Include(x => x.BidOfContractors)
                  .Include(x => x.ApplicationUser)
                  .FirstOrDefaultAsync(cancellationToken);
                if (contractor is null)
                {
                    return new Result<ContractorDto>().WithValue(null).Failure(ErrorMessages.ClientNotFound);
                }
                else
                {
                    var contractorDto = new ContractorDto
                    {
                        Id = contractor.Id,
                        Address = contractor.Address,
                        CompanyName = contractor.CompanyName,
                        Name = contractor.Name,
                        Email = contractor.Email,
                        FaxNumber = contractor.FaxNumber,
                        LandlineNumber = contractor.LandlineNumber,
                        MobileNumber = contractor.MobileNumber,
                        ApplicationUserId = contractor.ApplicationUserId,
                        BidOfContractors = contractor.BidOfContractors.Select(b => new BidOfContractorDto
                        {
                            Id = b.Id,
                            CanChangeBid = b.CanChangeBid,
                            SuggestedFee = b.SuggestedFee,
                            IsAccepted = b.IsAccepted,
                            RequestId = b.RequestId,
                            IsDeleted = b.IsDeleted,
                            CreatedAt = b.CreatedAt,
                            CreatedBy = b.CreatedBy,
                            UpdatedAt = b.UpdatedAt,
                            UpdatedBy = b.UpdatedBy,
                            DeletedAt = b.DeletedAt,
                            DeletedBy = b.DeletedBy,
                            ContractorId = b.ContractorId,


                        }).ToList(),
                    };
                    return new Result<ContractorDto>().WithValue(contractorDto).Success(SuccessMessages.ClientFound);
                }
            }
            catch (Exception ex)
            {
                return new Result<ContractorDto>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<AddRejectedRequestDto>> RejectRequest(AddRejectedRequestDto rejectedRequestDto, CancellationToken cancellationToken)
        {


        }
    }
}
