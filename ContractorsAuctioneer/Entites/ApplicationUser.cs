
using Microsoft.AspNetCore.Identity;

namespace ContractorsAuctioneer.Entites
{
    public class ApplicationUser : IdentityUser<int>
    {
        public Contractor? Contractor { get; set; }
        public Client? Client { get; set; }
        public List<VerificationCode>? VerificationCodes { get; set; }
    }
}
