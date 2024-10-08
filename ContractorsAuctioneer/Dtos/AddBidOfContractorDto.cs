using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddBidOfContractorDto : BaseAddAuditableDto
    {
        public int Id { get; set; }
        public int? SuggestedFee { get; set; }
        public int ContractorId { get; set; }
        public int RequestId { get; set; }
        public bool? IsAccepted { get; set; }
        public bool? CanChangeBid { get; set; }

    }
}
