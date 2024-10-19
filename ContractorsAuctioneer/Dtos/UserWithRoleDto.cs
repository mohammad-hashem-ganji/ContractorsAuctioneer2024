using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class UserWithRoleDto
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
