using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddBidStatusDto 
    {
        public int BidOfContractorId { get; set; }
        public BidStatusEnum Status { get; set; }
        public int CreatedBy { get; set; }
    }
}
