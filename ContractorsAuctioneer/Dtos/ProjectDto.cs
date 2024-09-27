using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public int ContractorBidId { get; set; }
        public BidOfContractor? ContractorBid { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public ICollection<ProjectStatus>? ProjectStatuses { get; set; }
    }
}
