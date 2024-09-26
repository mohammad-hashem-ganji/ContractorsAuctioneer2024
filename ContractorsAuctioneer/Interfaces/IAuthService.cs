using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IAuthService
    {
        Task<Result<RegisterResult>> RegisterAsync(string username, string password, string role);
        Task<ApplicationUser> AuthenticateAsync(string username, string password);
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
