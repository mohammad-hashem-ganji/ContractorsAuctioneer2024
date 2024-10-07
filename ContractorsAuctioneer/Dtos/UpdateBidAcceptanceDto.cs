namespace ContractorsAuctioneer.Dtos
{
    public class UpdateBidAcceptanceDto : BaseUpdateAuditableDto
    {
        public int BidId { get; set; }
        public int RequestId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
