using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsAuctioneer.Entites
{
    public class ProjectStatusHistory
    {
        public int Id { get; set; }
        public int ProjectStatusId { get; set; }
        public ProjectStatus? ProjectStatus { get; set; }

    }
}

