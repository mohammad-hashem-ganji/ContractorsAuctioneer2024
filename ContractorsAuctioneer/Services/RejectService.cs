﻿using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Claims;

namespace ContractorsAuctioneer.Services
{
    public class RejectService : IRejectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RejectService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<AddReasonToRejectRequestDto>> AddRjectRquestAsync(AddReasonToRejectRequestDto rejectRequestDto, CancellationToken cancellationToken)
        {
            try
            {
                if (rejectRequestDto is null)
                {
                    return new Result<AddReasonToRejectRequestDto>().WithValue(null).Failure("ورودی نامعتبر");
                }
                var isAppUserIdConvert = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int appUserId);
                if (!isAppUserIdConvert)
                {
                    return new Result<AddReasonToRejectRequestDto>().WithValue(null).Failure("خطا");
                }
                var rejectRequest = new Reject
                {
                    UserId = appUserId,
                    RequestId = rejectRequestDto.RequestId,
                    Reason = rejectRequestDto.Reason,
                    Comment = rejectRequestDto.Comment,
                    CreatedAt = DateTime.Now,
                    CreatedBy = appUserId
                };
                await _context.Rejects.AddAsync(rejectRequest, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<AddReasonToRejectRequestDto>().WithValue(rejectRequestDto).Success("دلیل رد پروژه ثبت شد.");
            }
            catch (Exception)
            {

                return new Result<AddReasonToRejectRequestDto>().WithValue(null).Failure("خطا");
            }


        }


        public async Task<Result<List<GetReasonOfRejectRequestDto>>> GetReasonsOfRejectingRequestByRequestIdAsync(int requestId, CancellationToken cancellationToken)
        {
            try
            {
                var reason = await _context.Rejects
                .Where(x => x.RequestId == requestId)
                .ToListAsync(cancellationToken);
                if (reason is null)
                {
                    return new Result<List<GetReasonOfRejectRequestDto>>().WithValue(null).Success("دلیلی پیدا نشد.");
                }
                List<GetReasonOfRejectRequestDto> reasonDto = reason.Select(r => new GetReasonOfRejectRequestDto
                {
                    Id = r.Id,
                    RequestId = r.RequestId,
                    Reason = r.Reason,
                    DateRejected = r.CreatedAt,
                    UserId = r.CreatedBy
                }).ToList();
                return new Result<List<GetReasonOfRejectRequestDto>>()
                    .WithValue(reasonDto)
                    .Success("دلیل‌های رد درخواست یافت شد.");
            }
            catch (Exception)
            {
                return new Result<List<GetReasonOfRejectRequestDto>>().WithValue(null).Failure("خطا!");
            }
            
        }


        public async Task<Result<List<GetReasonOfRejectRequestDto>>> GetReasonsOfRejectingRequestByClientAsycn(CancellationToken cancellationToken)
        {
            try
            {
                var isAppUserIdConvert = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out int appUserId);
                if (!isAppUserIdConvert)
                {
                    return new Result<List<GetReasonOfRejectRequestDto>>().WithValue(null).Failure("خطا");
                }
                var reason = await _context.Rejects
                    .Where(x => x.UserId == appUserId)
                    .ToListAsync(cancellationToken);
                if (reason is null)
                {
                    return new Result<List<GetReasonOfRejectRequestDto>>().WithValue(null).Success("دلیلی پیدا نشد.");
                }
                List<GetReasonOfRejectRequestDto> reasonDto = reason.Select(r => new GetReasonOfRejectRequestDto
                {
                    Id = r.Id,
                    RequestId = r.RequestId,
                    Reason = r.Reason,
                    DateRejected = r.CreatedAt,
                    UserId = r.CreatedBy
                }).ToList();
                return new Result<List<GetReasonOfRejectRequestDto>>()
                    .WithValue(reasonDto)
                    .Success("دلیل‌های رد درخواست یافت شد.");

            }
            catch (Exception)
            {
                return new Result<List<GetReasonOfRejectRequestDto>>().WithValue(null).Failure("خطا!");
            }
        }
    }
}
