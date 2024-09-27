using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IClientService _clientService;
        private readonly IRegionService _regionService;
        private readonly IAuthService _authService;

        public RequestService(ApplicationDbContext context, IClientService clientService, IRegionService regionService, IAuthService authService)
        {
            _context = context;
            _clientService = clientService;
            _regionService = regionService;
            _authService = authService;
        }




        public async Task<bool> AddAsync(AddRequestDto requestDto, CancellationToken cancellationToken)
        {
            string role = "Client";

            if (requestDto == null)
            {
                // log
                return false;
            }
            try
            {
                var applicationUserId = await _authService.RegisterAsync(requestDto.Username, requestDto.Password, role);
                if (applicationUserId.Data.RegisteredUserId == 0)
                {
                    return false;
                }
                var clientId = await _clientService.AddAsync(new Client
                {
                    NCcode = requestDto.Client.NCcode,
                    MainSection = requestDto.Client.MainSection,
                    SubSection = requestDto.Client.SubSection,
                    address = requestDto.Client.address,
                    LicensePlate = requestDto.Client.LicensePlate,
                    IsDeleted = false,
                    DeletedBy = null,
                    DeletedAt = null,
                    ApplicationUserId = applicationUserId.Data.RegisteredUserId,                   
                }, cancellationToken);
                if (clientId == 0 )
                {
                    return false;
                }
                var regionId = await _regionService.AddAsync(new Region
                {
                    Title = requestDto.Region.Title,
                    ContractorSystemCode = requestDto.Region.ContractorSystemCode,
                },cancellationToken);
                var request = new Request
                {
                    Title = requestDto.Title,
                    Description = requestDto.Description,
                    RegistrationDate = requestDto.RegistrationDate,
                    ConfirmationDate = requestDto.ConfirmationDate,
                    CanChangeOrder = requestDto.CanChangeOrder,
                    RegionId = regionId,
                    ClientId = clientId,
                    CreatedAt = requestDto.CreatedAt,
                    RequestNumber = requestDto.RequestNumber,
                };
                await _context.Requests.AddAsync(request, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                //log
                return false;
            }

        }
        public async Task<Result<List<RequestDto>>?> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var requests = await _context.Requests
                   .Include(x => x.Client)
                   .Include(x => x.Region)
                   .Include(x => x.RequestStatuses)
                   .Include(x => x.FileAttachments)
                   .Include(x => x.BidOfContractors)
                   .ToListAsync(cancellationToken);
                var requestDtos = requests.Select(x => new RequestDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    RegistrationDate = x.RegistrationDate,
                    ConfirmationDate = x.ConfirmationDate,
                    CanChangeOrder = x.CanChangeOrder,
                    ClientId = x.ClientId,
                    RegionId = x.RegionId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    DeletedAt = x.DeletedAt,
                    DeletedBy = x.DeletedBy,
                    IsDeleted = x.IsDeleted,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                    RequestStatuses = x.RequestStatuses.Select(rs => new RequestStatusDto
                    {
                        Id = rs.Id,
                        Status = rs.Status,
                        CreatedAt = rs.CreatedAt,
                        CreatedBy = rs.CreatedBy,
                        DeletedAt = rs.DeletedAt,
                        DeletedBy = rs.DeletedBy,
                        IsDeleted = rs.IsDeleted,
                        UpdatedAt = rs.UpdatedAt,
                        UpdatedBy = rs.UpdatedBy
                    }).ToList(),
                    BidOfContractors = x.BidOfContractors.Select(b => new BidOfContractorDto
                    {
                        Id = b.Id,
                        SuggestedFee = b.SuggestedFee,
                        ContractorId = b.ContractorId,
                        IsAccepted = b.IsAccepted,
                        CanChangeBid = b.CanChangeBid,
                        CreatedAt = b.CreatedAt,
                        CreatedBy = b.CreatedBy,
                        DeletedAt = b.DeletedAt,
                        DeletedBy = b.DeletedBy,
                        IsDeleted = b.IsDeleted,
                        UpdatedAt = b.UpdatedAt,
                        UpdatedBy = b.UpdatedBy
                    }).ToList(),
                    FileAttachments = x.FileAttachments.Select(f => new FileAttachmentDto
                    {
                        Id = f.Id,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        CreatedAt = f.CreatedAt,
                        CreatedBy = f.CreatedBy,
                        DeletedAt = f.DeletedAt,
                        DeletedBy = f.DeletedBy,
                        IsDeleted = f.IsDeleted,
                        UpdatedAt = f.UpdatedAt,
                        UpdatedBy = f.UpdatedBy
                    }).ToList()
                }).ToList();

                return new Result<List<RequestDto>>().WithValue(requestDtos);
            }
            catch (Exception ex)
            {
                return new Result<List<RequestDto>>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<RequestDto>> GetByIdAsync(int reqId, CancellationToken cancellationToken)
        {
            try
            {
                var request = await _context.Requests
                   .Where(x => x.Id == reqId)
                   .Include(x => x.Client)
                   .Include(x => x.Region)
                   .Include(x => x.RequestStatuses)
                   .Include(x => x.FileAttachments)
                   .Include(x => x.BidOfContractors)
                   .FirstOrDefaultAsync(cancellationToken);
                if (request == null)
                {
                    return new Result<RequestDto>().WithValue(null).Failure(ErrorMessages.RequestNotFound);
                }
                var requestDto = new RequestDto
                {
                    Id = request.Id,
                    Title = request.Title,
                    Description = request.Description,
                    RegistrationDate = request.RegistrationDate,
                    ConfirmationDate = request.ConfirmationDate,
                    CanChangeOrder = request.CanChangeOrder,
                    ClientId = request.ClientId,
                    RegionId = request.RegionId,
                    CreatedAt = request.CreatedAt,
                    CreatedBy = request.CreatedBy,
                    DeletedAt = request.DeletedAt,
                    DeletedBy = request.DeletedBy,
                    IsDeleted = request.IsDeleted,
                    UpdatedAt = request.UpdatedAt,
                    UpdatedBy = request.UpdatedBy,
                    RequestStatuses = request.RequestStatuses.Select(rs => new RequestStatusDto
                    {
                        Id = rs.Id,
                        Status = rs.Status,
                        CreatedAt = rs.CreatedAt,
                        CreatedBy = rs.CreatedBy,
                        DeletedAt = rs.DeletedAt,
                        DeletedBy = rs.DeletedBy,
                        IsDeleted = rs.IsDeleted,
                        UpdatedAt = rs.UpdatedAt,
                        UpdatedBy = rs.UpdatedBy
                    }).ToList(),
                    BidOfContractors = request.BidOfContractors.Select(b => new BidOfContractorDto
                    {
                        Id = b.Id,
                        SuggestedFee = b.SuggestedFee,
                        ContractorId = b.ContractorId,
                        IsAccepted = b.IsAccepted,
                        CanChangeBid = b.CanChangeBid,
                        CreatedAt = b.CreatedAt,
                        CreatedBy = b.CreatedBy,
                        DeletedAt = b.DeletedAt,
                        DeletedBy = b.DeletedBy,
                        IsDeleted = b.IsDeleted,
                        UpdatedAt = b.UpdatedAt,
                        UpdatedBy = b.UpdatedBy
                    }).ToList(),
                    FileAttachments = request.FileAttachments.Select(f => new FileAttachmentDto
                    {
                        Id = f.Id,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        CreatedAt = f.CreatedAt,
                        CreatedBy = f.CreatedBy,
                        DeletedAt = f.DeletedAt,
                        DeletedBy = f.DeletedBy,
                        IsDeleted = f.IsDeleted,
                        UpdatedAt = f.UpdatedAt,
                        UpdatedBy = f.UpdatedBy
                    }).ToList()
                };

                return new Result<RequestDto>().WithValue(requestDto).Success(SuccessMessages.Regionfound);
            }
            catch (Exception ex)
            {
                return new Result<RequestDto>().WithValue(null).Failure(ex.Message);
            }
        }

 
     

    }
}
