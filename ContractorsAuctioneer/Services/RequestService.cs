﻿using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
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
        private readonly IContractorService _contractorService;
        private readonly IClientService _clientService;
        private readonly IRegionService _regionService;
        private readonly IAuthService _authService;
        private readonly IFileAttachmentService _fileAttachmentService;
        public RequestService(ApplicationDbContext context, IClientService clientService, IRegionService regionService, IAuthService authService, IFileAttachmentService fileAttachmentService, IContractorService contractorService)
        {
            _context = context;
            _clientService = clientService;
            _regionService = regionService;
            _authService = authService;
            _fileAttachmentService = fileAttachmentService;
            _contractorService = contractorService;
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
                if (clientId == 0)
                {
                    return false;
                }
                var regionId = await _regionService.AddAsync(new Region
                {
                    Title = requestDto.Region.Title,
                    ContractorSystemCode = requestDto.Region.ContractorSystemCode,
                }, cancellationToken);
                var request = new Entites.Request
                {
                    Title = requestDto.Title,
                    Description = requestDto.Description,
                    RegistrationDate = requestDto.RegistrationDate,
                    ConfirmationDate = requestDto.ConfirmationDate,
                    IsFileCheckedByClient = false,
                    IsActive = true,
                    ExpireAt = DateTime.Now.AddDays(3),
                    RegionId = regionId,
                    ClientId = clientId,
                    CreatedAt = requestDto.CreatedAt,
                    RequestNumber = requestDto.RequestNumber,
                    IsTenderOver = false,
                    IsDeleted = false
                };


                await _context.Requests.AddAsync(request, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                // Add fillAttachment ### here
                await _fileAttachmentService.AddAsync(new FileUploadDto
                {
                    File = requestDto.FileUploadDto.File,
                    FileAttachmentType = requestDto.FileUploadDto.FileAttachmentType,
                    RequestId = request.Id,
                }, cancellationToken);
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
                    ClientId = x.ClientId,
                    RegionId = x.RegionId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    DeletedAt = x.DeletedAt,
                    DeletedBy = x.DeletedBy,
                    IsDeleted = x.IsDeleted,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                    FileAttachments = x.FileAttachments
                    .Where(f => f.IsDeleted == false)
                    .Select(f => new FileAttachmentDto
                    {
                        Id = f.Id
                    }).ToList()
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
                   .Where(x =>
                   x.Id == requestId && x.IsTenderOver == false && x.IsActive == true)
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
        public async Task<Result<RequestDto>> GetRequestOfClientAsync(int clientId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _context.Requests
                   .Where(x =>
                   x.ClientId == clientId && x.IsTenderOver == false && x.IsActive == true && x.IsAcceptedByClient == false)
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
                       IsActive = x.IsActive,
                       ExpireAt = x.ExpireAt,
                       IsAcceptedByClient = x.IsAcceptedByClient,
                       IsTenderOver = x.IsTenderOver,
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
                       }).ToList(),
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

        public async Task<Result<List<RequestDto>>> GetRequestsforContractor(int contractorId, CancellationToken cancellationToken)
        {
            try
            {
                var requests = await _context.Requests
                   .Where(x => x.IsTenderOver == false && x.IsActive == true && x.RequestStatuses
                   .All(status => status.Status != RequestStatusEnum.RequestRejectedByContractor 
                   && status.RequestId == x.Id 
                   && status.CreatedBy == contractorId))
                   .Include(x => x.FileAttachments) 
                   .ToListAsync(cancellationToken);
                var requestDtos = requests.Select(x => new RequestDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ClientId = x.ClientId,
                    RegionId = x.RegionId,
                    FileAttachments = x.FileAttachments
                    .Where(f => f.IsDeleted == false)
                    .Select(f => new FileAttachmentDto
                    {
                        Id = f.Id
                    }).ToList()
                }).ToList();
                if (requestDtos.Any())
                {
                    return new Result<List<RequestDto>>()
                        .WithValue(requestDtos)
                        .Success("درخواست ها یافت شدند .");
                }
                else
                {

                    return new Result<List<RequestDto>>()
                        .WithValue(requestDtos)
                        .Failure("درخواستی وجود ندارد");
                }
            }
            catch (Exception ex)
            {
                return new Result<List<RequestDto>>()
                    .WithValue(null)
                    .Failure("هنگام اجرا خطایی پیش آمد!");
            }
        }


        public async Task<Result<RequestDto>> UpdateAsync(RequestDto requestDto, CancellationToken cancellationToken)
        {
            try
            {
                Entites.Request? request = await _context.Requests
                .Where(x =>
                x.Id == requestDto.Id && x.IsActive == true && x.IsTenderOver == false)
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
                request.IsAcceptedByClient = requestDto.IsAcceptedByClient;
                request.ExpireAt = requestDto.ExpireAt;
                request.IsTenderOver = requestDto.IsTenderOver;
                request.IsActive = requestDto.IsActive;
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
