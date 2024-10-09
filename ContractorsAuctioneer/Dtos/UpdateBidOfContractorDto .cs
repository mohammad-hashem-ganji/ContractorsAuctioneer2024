﻿using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class UpdateBidOfContractorDto : BaseUpdateAuditableDto
    {
        public int Id { get; set; }
        public int? SuggestedFee { get; set; }
        public int RequestId { get; set; }
        public bool? CanChangeBid { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ExpireAt { get; set; }
        public ICollection<BidStatus>? BidStatuses { get; set; }
    }
}