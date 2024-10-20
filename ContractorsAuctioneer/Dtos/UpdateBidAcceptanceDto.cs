namespace ContractorsAuctioneer.Dtos
{
    public class UpdateBidAcceptanceDto 
    {
        public int BidId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
