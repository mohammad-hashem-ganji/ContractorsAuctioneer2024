
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsAuctioneer.Entites
{
    public class BidStatusHistory : BaseAuditableEntity
    {
        public int Id { get; set; }
        public int BidStatusId { get; set; }
        public BidStatus? BidStatus { get; set; }

    }
}
