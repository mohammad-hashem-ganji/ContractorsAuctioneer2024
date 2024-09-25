
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsAuctioneer.Entites
{
    public class BidStatus
    {
        public int Id { get; set; }
        public int ContractorBidId { get; set; }
        public BidOfContractor? ContractorBid { get; set; }
        public BidStatusEnum? Status { get; set; }
        public ICollection<BidStatusHistory>? BidStatusHistories { get; set; }

    }
}
