using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class FileUploadDto
    {
        public IFormFile File { get; set; }
        public int RequestId { get; set; }
        public FileAttachmentType FileAttachmentType{ get; set; }
    }
}
