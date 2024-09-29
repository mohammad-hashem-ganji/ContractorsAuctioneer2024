using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using Azure.Core;
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
                var applicationUserResult = await _authService.RegisterAsync(requestDto.Username, requestDto.Password, role);
                if (applicationUserResult.Data.RegisteredUserId == 0)
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
                    ApplicationUserId = applicationUserResult.Data.RegisteredUserId,                   
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
                var request = new Entites.Request
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
                    IsTendrOver = false,
                    IsDeleted = false
                };
                await _context.Requests.AddAsync(request, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                // Add fillAttachment ### here
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
                        UpdatedAt = rs.UpdatedAt,
                        UpdatedBy = rs.UpdatedBy
                    }).FirstOrDefault(),
                    //BidOfContractors = x.BidOfContractors.Select(b => new BidOfContractorDto
                    //{
                    //    Id = b.Id,
                    //    SuggestedFee = b.SuggestedFee,
                    //    ContractorId = b.ContractorId,
                    //    IsAccepted = b.IsAccepted,
                    //    CanChangeBid = b.CanChangeBid,
                    //    CreatedAt = b.CreatedAt,
                    //    CreatedBy = b.CreatedBy,
                    //    DeletedAt = b.DeletedAt,
                    //    DeletedBy = b.DeletedBy,
                    //    IsDeleted = b.IsDeleted,
                    //    UpdatedAt = b.UpdatedAt,
                    //    UpdatedBy = b.UpdatedBy
                    //}).ToList(),
                    //FileAttachments = x.FileAttachments.Select(f => new FileAttachmentDto
                    //{
                    //    Id = f.Id,
                    //    FileName = f.FileName,
                    //    FilePath = f.FilePath,
                    //    CreatedAt = f.CreatedAt,
                    //    CreatedBy = f.CreatedBy,
                    //    DeletedAt = f.DeletedAt,
                    //    DeletedBy = f.DeletedBy,
                    //    IsDeleted = f.IsDeleted,
                    //    UpdatedAt = f.UpdatedAt,
                    //    UpdatedBy = f.UpdatedBy
                    //}).ToList()
                }).ToList();
                if (requestDtos.Any())
                {
                    return new Result<List<RequestDto>>().WithValue(requestDtos).Success("درخواست ها یافت شدند .");
                }
                else
                {

                return new Result<List<RequestDto>>().WithValue(requestDtos).Failure("درخواستی وجود ندارد");
                }
            }
            catch (Exception ex)
            {
                return new Result<List<RequestDto>>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<RequestDto>> GetByIdAsync(int requestId, CancellationToken cancellationToken)
        {
            try
            {
                var request = await _context.Requests
                   .Where(x => x.Id == requestId)
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
                        UpdatedAt = rs.UpdatedAt,
                        UpdatedBy = rs.UpdatedBy
                    }).FirstOrDefault(),
                    //BidOfContractors = request.BidOfContractors.Select(b => new BidOfContractorDto
                    //{
                    //    Id = b.Id,
                    //    SuggestedFee = b.SuggestedFee,
                    //    ContractorId = b.ContractorId,
                    //    IsAccepted = b.IsAccepted,
                    //    CanChangeBid = b.CanChangeBid,
                    //    CreatedAt = b.CreatedAt,
                    //    CreatedBy = b.CreatedBy,
                    //    DeletedAt = b.DeletedAt,
                    //    DeletedBy = b.DeletedBy,
                    //    IsDeleted = b.IsDeleted,
                    //    UpdatedAt = b.UpdatedAt,
                    //    UpdatedBy = b.UpdatedBy
                    //}).ToList(),
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
        public async Task<Result<RequestDto>> GetRequestOfClientAsync(int clientId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _context.Requests
                   .Where(x => x.ClientId == clientId && x.IsTendrOver == false && x.IsDeleted == false)
                   .Include(x => x.Client)
                   .Include(x => x.Region)
                   .Include(x => x.RequestStatuses)
                   .Include(x => x.FileAttachments)
                   .Include(x => x.BidOfContractors)
                   .Select(x => new RequestDto
                   {
                       Id = x.Id,
                       Title = x.Title,
                       Description = x.Description,
                       RegistrationDate = x.RegistrationDate,
                       ConfirmationDate = x.ConfirmationDate,
                       CanChangeOrder = x.CanChangeOrder,
                       ClientId = x.ClientId,
                       RegionId = x.RegionId,
                       RequestNumber = x.RequestNumber,
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
                           UpdatedAt = rs.UpdatedAt,
                           UpdatedBy = rs.UpdatedBy
                       }).FirstOrDefault(),
                       BidOfContractors = x.BidOfContractors.Select(b => new BidOfContractorDto
                       {
                           Id = b.Id,
                           SuggestedFee = b.SuggestedFee,
                           ContractorId = b.ContractorId,
                           IsAccepted = b.IsAccepted,
                           CreatedAt = b.CreatedAt,
                       }).ToList(),
                       FileAttachments = x.FileAttachments.Select(f => new FileAttachmentDto
                       {
                           Id = f.Id,
                           FileName = f.FileName,
                           FilePath = f.FilePath,
                           IsDeleted = f.IsDeleted,
                       }).ToList()

                   }).FirstOrDefaultAsync(cancellationToken);
                if (result is not null)
                {
                    return new Result<RequestDto>().WithValue(result).Success("درخواست ها یافت شدند .");
                }
                else
                {

                    return new Result<RequestDto>().WithValue(result).Failure("درخواستی وجود ندارد");
                }
            }
            catch (Exception ex)
            {
                return new Result<RequestDto>().WithValue(null).Failure(ex.Message);
            }

        }
        public async Task<Result<RequestDto>> UpdateAsync(RequestDto requestDto, CancellationToken cancellationToken)
        {
            try
            {
                Entites.Request? request = await _context.Requests
                .Where(x => x.Id == requestDto.Id)
                .FirstOrDefaultAsync(cancellationToken);
                if (request is null)
                {
                    return new Result<RequestDto>().WithValue(null).Failure(ErrorMessages.RequestNotFound);
                }
                request.Title = requestDto.Title;
                request.RequestNumber = requestDto.RequestNumber;
                request.RegistrationDate = requestDto.RegistrationDate;
                request.ConfirmationDate = request.ConfirmationDate;
                request.Description = requestDto.Description;
                request.UpdatedAt = requestDto.UpdatedAt;
                request.UpdatedBy = requestDto.UpdatedBy;
                _context.Requests.Update(request);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<RequestDto>().WithValue(requestDto).Success("درخواست آپدیت شد");
            }
            catch (Exception ex)
            {
                return new Result<RequestDto>().WithValue(null).Failure(ex.Message);
            }
            





        }
 
     

    }
}
