
using Microsoft.AspNetCore.Identity;

namespace ContractorsAuctioneer.Entites
{
    public class ApplicationUser : IdentityUser<int>
    {
        public Contractor? Contractor { get; set; }
        public Client? Client { get; set; }
        public List<LastLoginHistory>? LastLoginHistories{ get; set; }
        public string? FirsName { get; set; }
        public string? LastName { get; set; }
    }
}
