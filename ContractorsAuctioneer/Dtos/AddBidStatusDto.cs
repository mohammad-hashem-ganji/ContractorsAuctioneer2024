using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddBidStatusDto : BaseAddAuditableDto
    {
        public int BidOfContractorId { get; set; }
        public BidStatusEnum Status { get; set; }
    }
}
