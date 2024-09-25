using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class RegionService
    {
        private readonly ApplicationDbContext _context;

        public RegionService(ApplicationDbContext? context)
        {
            _context = context;
        }
        public async Task<int> AddAsync(Region region, CancellationToken cancellationToken)
        {
            await _context.Regions.AddAsync(region, cancellationToken);
            var trackeNum = await _context.SaveChangesAsync(cancellationToken);
            if (trackeNum >= 1)
            {
                return region.Id;
            }
            else
            {
                return 0;
            }
        }
        public async Task<Result<RegionDto>> GetByIdAsync(int regionId, CancellationToken cancellationToken)
        {
            try
			{
                var region = await _context.Regions
              .Where(x => x.Id == regionId)
              .Include(x => x.Requests)
              .FirstOrDefaultAsync(cancellationToken);
                if (region is null)
                {
                    return new Result<RegionDto>().WithValue(null).Failure(ErrorMessages.RegionNotFound);
                }
                else
                {
                    var regionDto = new RegionDto
                    {
                        Id = region.Id,
                        Title = region.Title,
                        ContractorSystemCode = region.ContractorSystemCode,
                        Requests = region.Requests.Select(rs => new RequestDto
                        {
                            Id = rs.Id,
                            Title = rs.Title,
                            ConfirmationDate = rs.ConfirmationDate,
                            RegistrationDate = rs.RegistrationDate,
                            CreatedAt = rs.CreatedAt,
                            CreatedBy = rs.CreatedBy,
                            Description = rs.Description,
                            IsDeleted = rs.IsDeleted,
                            DeletedAt = rs.DeletedAt,
                            DeletedBy = rs.DeletedBy,
                            CanChangeOrder = rs.CanChangeOrder,
                            UpdatedAt = rs.UpdatedAt,
                            UpdatedBy = rs.UpdatedBy

                        }).ToList(),
                    };
                    return new Result<RegionDto>().WithValue(regionDto).Success(SuccessMessages.Regionfound);
                }
            }
			catch (Exception ex)
			{
                return new Result<RegionDto>().WithValue(null).Failure(ex.Message);
            }
        }
    }
}
