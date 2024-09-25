using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsAuctioneer.Entites
{
    public class RequestStatusHistory
    {
        public int Id { get; set; }
        public int RequestStatusId { get; set; }
        public RequestStatus? RequestStatus { get; set; }

    }
}
