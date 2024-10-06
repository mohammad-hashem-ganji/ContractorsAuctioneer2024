using System.ComponentModel.DataAnnotations;

namespace ContractorsAuctioneer.Dtos
{
    public class UpdateRejectedRequestDto : BaseUpdateAuditableDto
    {
        [Required]
        public int RequestId { get; set; }
        [Required]
        public bool IsAccepted { get; set; }

    }
}
