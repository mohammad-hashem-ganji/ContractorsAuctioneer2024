using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddRegionDto : BaseAddAuditableDto
    {
        public string? Title { get; set; }
        public int? ContractorSystemCode { get; set; }
    }
}
