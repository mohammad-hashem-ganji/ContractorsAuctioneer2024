﻿using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;

namespace ContractorsAuctioneer.Services
{
    public class FileAttachmentService : IFileAttachmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public FileAttachmentService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<Result<FileAttachmentDto>> AddAsync(FileUploadDto model, CancellationToken cancellationToken)
        {
            try
            {
                string path = string.Empty;
                if (model is null)
                {
                    return new Result<FileAttachmentDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
                }
                const long maxFileSize = 7 * 1024 * 1024;
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                if (model.File.Length > maxFileSize)
                {
                    return new Result<FileAttachmentDto>()
                        .WithValue(null)
                        .Failure("حجم فایل زیاد است.");
                }
                var fileExtension = Path.GetExtension(model.File.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new Result<FileAttachmentDto>()
                        .WithValue(null)
                        .Failure("File type is not allowed.");
                }
                if (model.FileAttachmentType == FileAttachmentType.PlanNotebook)
                {
                    path = Path.Combine(_environment.WebRootPath, "FileAttachments", "planNotebooks");
                    var a = 0;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                if (model.FileAttachmentType == FileAttachmentType.Other)
                {
                    path = Path.Combine(_environment.WebRootPath, "FileAttachments","Others");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                Guid newguid = Guid.NewGuid();
                string fileName = $"{newguid}_{model.File.FileName}";
                var filePath = Path.Combine(path, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream, cancellationToken);
                }
                var fileAttachment = new FileAttachment
                {
                    FileName = model.File.FileName,
                    FilePath = filePath,
                    RequestId = model.RequestId
                };
                _context.FileAttachments.Add(fileAttachment);
                await _context.SaveChangesAsync(cancellationToken);

                // Return the result
                return new Result<FileAttachmentDto>()
                    .WithValue(null)
                    .Success("فایل آپلود شد");
            }
            catch (Exception ex)
            {
                return new Result<FileAttachmentDto>()
                    .WithValue(null)
                    .Failure(ex.Message);
            }



        }








    }
}