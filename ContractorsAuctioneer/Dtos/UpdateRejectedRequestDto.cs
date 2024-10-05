using System.ComponentModel.DataAnnotations;

namespace ContractorsAuctioneer.Dtos
{
    public class UpdateRejectedRequestDto : BaseAddAuditableDto
    {
        [Required]
        public string Comment { get; set; }
        [Required]
        public int RequestId { get; set; }
        [Required]
        public bool IsAccepted { get; set; }
        public int ClientId { get; set; }
    }
}
