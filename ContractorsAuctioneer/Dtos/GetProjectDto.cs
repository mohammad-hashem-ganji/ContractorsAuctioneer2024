﻿using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class GetProjectDto : BaseAuditableEntity
    {
        public int Id { get; set; }
        public int ContractorBidId { get; set; }
        public BidOfContractor? ContractorBid { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public ProjectStatus? ProjectStatuses { get; set; }
    }
}
