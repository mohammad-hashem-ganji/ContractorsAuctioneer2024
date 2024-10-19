using ContractorsAuctioneer.Entites;

namespace ContractorsAuctioneer.Dtos
{
    public class UserWithRoleAndTokenDto
    {
        public IList<string> Roles { get; set; }
        public string Token { get; set; }
    }
}
