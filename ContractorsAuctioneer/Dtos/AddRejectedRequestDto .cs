namespace ContractorsAuctioneer.Dtos
{
    public class AddRejectedRequestDto : BaseAddAuditableDto
    {
        public int RequestId { get; set; }
        public int ContractorId { get; set; }
    }
}
