using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class ProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<AddProjectDto>> AddAsync(AddProjectDto addProjectDto, CancellationToken cancellationToken)
        {
            if (addProjectDto == null)
            {
                return new Result<AddProjectDto>().WithValue(null).Failure(ErrorMessages.EntityIsNull);
            }
            try
            {
                var project = new Project
                {
                    ContractorBidId = addProjectDto.ContractorBidId,
                };
                await _context.Projects.AddAsync(project, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new Result<AddProjectDto>().WithValue(addProjectDto).Success(SuccessMessages.OperationSuccessful);
            }
            catch (Exception ex)
            {
                return new Result<AddProjectDto>().WithValue(null).Failure(ex.Message);
            }
        }
        public async Task<Result<GetProjectDto>> GetByIdAsync(int projectId , CancellationToken cancellationToken)
        {
            try
            {
                Project? project = await _context.Projects
                    .Where(x => x.Id == projectId)
                    .Include(x => x.ContractorBid)
                    .Include(x => x.ProjectStatuses)
                    .FirstOrDefaultAsync(cancellationToken);
                if (project == null)
                {
                    return new Result<GetProjectDto>().WithValue(null).Failure(ErrorMessages.ProjectNotFound);
                }
                else
                {
                    var projectDto = new GetProjectDto
                    {
                        Id = project.Id,
                        ContractorBidId = project.ContractorBidId,
                        StartedAt = project.StartedAt,
                        CompletedAt = project.CompletedAt,
                        ProjectStatuses = project.ProjectStatuses.Select(p => new ProjectStatus
                        {
                            Id = p.Id,
                            Status = p.Status,
                            CreatedAt = p.CreatedAt,
                            CreatedBy = p.CreatedBy,
                            UpdatedAt = p.UpdatedAt,
                            UpdatedBy = p.UpdatedBy,
                            DeletedAt = p.DeletedAt,
                            DeletedBy = p.DeletedBy,
                        }).ToList(),

                    };
                    return new Result<GetProjectDto>().WithValue(projectDto).Success("پروژه پیدا شد");
                }
            }
            catch (Exception ex)
            {
                return new Result<GetProjectDto>().WithValue(null).Failure(ex.Message);
            }
        }




    }
}
