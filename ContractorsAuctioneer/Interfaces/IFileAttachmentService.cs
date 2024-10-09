using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IFileAttachmentService
    {
        Task<Result<FileAttachmentDto>> AddAsync(FileUploadDto model, CancellationToken cancellationToken);
        Task<Result<FileStreamResult>> GetFileAsync(int fileId, CancellationToken cancellationToken);
        Task<Result<UpdateFileAttachmentDto>> UpdateAsync(UpdateFileAttachmentDto fileAttachmentDto, CancellationToken cancellationToken);
    }
}
