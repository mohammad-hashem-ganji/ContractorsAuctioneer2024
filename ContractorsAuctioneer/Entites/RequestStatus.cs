
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsAuctioneer.Entites
{
    public class RequestStatus : BaseAuditableEntity
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public Request? Request { get; set; }
        public RequestStatusEnum? Status { get; set; }
        public ICollection<RequestStatusHistory>? RequestStatusHistories { get; set; }

    }
}
