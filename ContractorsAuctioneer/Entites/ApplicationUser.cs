
using Microsoft.AspNetCore.Identity;

namespace ContractorsAuctioneer.Entites
{
    public class ApplicationUser : IdentityUser<int>
    {
        public Contractor? Contractor { get; set; }
        public Client? Client { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? Last2FaAuthentication { get; set; }
        public List<VerificationCode>? VerificationCodes { get; set; }
        public List<LastLoginHistory>? LastLoginHistories{ get; set; }
    }
}
