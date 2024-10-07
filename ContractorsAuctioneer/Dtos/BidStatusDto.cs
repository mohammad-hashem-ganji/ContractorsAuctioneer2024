using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class BidStatusDto : BaseGetAuditaleDto
    {
        public int BidOfContractorId { get; set; }
        public BidStatusEnum Status { get; set; }
    }
}
