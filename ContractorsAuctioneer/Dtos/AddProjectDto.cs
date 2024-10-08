using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class AddProjectDto : BaseAddAuditableDto
    {
        public int Id { get; set; }
        public int ContractorBidId { get; set; }

    }
}
