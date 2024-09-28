using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsAuctioneer.Entites
{
    public class Request : BaseAuditableEntity
    {
        public int Id { get; set; }
        public string? RequestNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public bool? CanChangeOrder { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public int RegionId { get; set; }
        public Region? Region { get; set; }
        public bool IsTendrOver { get; set; }
        public ICollection<FileAttachment>? FileAttachments { get; set; }
        public ICollection<RequestStatus>? RequestStatuses { get; set; }
        public ICollection<BidOfContractor>? BidOfContractors { get; set; }

    }
}
