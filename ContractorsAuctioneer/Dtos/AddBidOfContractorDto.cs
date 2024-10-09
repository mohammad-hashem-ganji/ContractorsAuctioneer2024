using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddBidOfContractorDto : BaseAddAuditableDto
    {
        public int? SuggestedFee { get; set; }
        public int ContractorId { get; set; }
        public int RequestId { get; set; }
    }
}
