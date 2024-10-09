namespace ContractorsAuctioneer.Dtos
{
    public class GetUpdateBidAcceptanceDto
    {
        public int Id { get; set; }
        public int? SuggestedFee { get; set; }
        public bool? CanChangeBid { get; set; }
        public bool IsAccepted { get; set; }
    }
}
