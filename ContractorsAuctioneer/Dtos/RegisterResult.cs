using Microsoft.AspNetCore.Identity;

namespace ContractorsAuctioneer.Dtos
{
    public class RegisterResult
    {
        public int RegisteredUserId { get; set; }
        public IdentityResult  IdentityResult { get; set; }
        public List<IdentityError> IdentityError { get; set; }
    }
}
