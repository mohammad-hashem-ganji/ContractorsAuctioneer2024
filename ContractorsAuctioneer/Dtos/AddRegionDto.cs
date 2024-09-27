using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddRegionDto : BaseAuditableEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? ContractorSystemCode { get; set; }
    }
}
