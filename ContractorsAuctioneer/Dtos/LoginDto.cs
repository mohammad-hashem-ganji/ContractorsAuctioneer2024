using System.ComponentModel.DataAnnotations;

namespace ContractorsAuctioneer.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Ncode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }
}
