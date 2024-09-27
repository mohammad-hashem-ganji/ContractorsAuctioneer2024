using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IRegionService
    {
        Task<int> AddAsync(Region region, CancellationToken cancellationToken);
        Task<Result<RegionDto>> GetByIdAsync(int regionId, CancellationToken cancellationToken);
    }
}
