namespace ContractorsAuctioneer.Dtos
{
    public class UpdateRequestAcceptanceDto : BaseUpdateAuditableDto
    {
        public int RequestId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
