using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class FileAttachmentDto : BaseAuditableEntity
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int RequestId { get; set; }

    }
}
