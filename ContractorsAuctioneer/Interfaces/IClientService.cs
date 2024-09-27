using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IClientService
    {
        Task<int> AddAsync(Client client, CancellationToken cancellationToken);
        Task<Result<ClientDto>> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
