using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Results;

namespace ContractorsAuctioneer.Interfaces
{
    public interface IProjectService
    {
        Task<Result<AddProjectDto>> AddAsync(AddProjectDto addProjectDto, CancellationToken cancellationToken);
    }
}
