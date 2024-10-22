using ContractorsAuctioneer.Entites;
using System.ComponentModel.DataAnnotations;

namespace ContractorsAuctioneer.Dtos
{
    public class AddReasonToRejectRequestDto 
    {
        [Required]
        public int RequestId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public RejectReasonEnum? Reason { get; set; }
    }
}
