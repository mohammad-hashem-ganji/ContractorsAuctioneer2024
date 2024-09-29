using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IFileAttachmentService
    {
        Task<Result<FileAttachmentDto>> AddAsync(FileUploadDto model, CancellationToken cancellationToken);
    }
}
