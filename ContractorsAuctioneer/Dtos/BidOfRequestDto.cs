using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class BidOfRequestDto
    {
        public int Id { get; set; }
        public int? SuggestedFee { get; set; }
        public int ContractorId { get; set; }
        public int RequestId { get; set; }
        public bool? IsAccepted { get; set; }
    }
}
