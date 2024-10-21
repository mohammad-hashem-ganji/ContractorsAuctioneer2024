using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using Azure.Core;
using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ContractorsAuctioneer.Services
{
    public class ProjectService : IProjectService
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
                var project = new Entites.Project
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
        public async Task<Result<GetProjectDto>> GetByIdAsync(int projectId, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
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
                            UpdatedAt = p.UpdatedAt,
                            UpdatedBy = p.UpdatedBy,
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



        public async Task<Result<GetProjectDto>> GetProjectOfbidAsync(int bidId, CancellationToken cancellationToken)
        {
            try
            {
                var bid = await _context.Projects
                    .Where(x => x.ContractorBidId == bidId )
                    .FirstOrDefaultAsync(cancellationToken);
                if (bid == null)
                {
                    return new Result<GetProjectDto>().WithValue(null).Failure(ErrorMessages.ProjectNotFound);
                }
                else
                {
                    var projectDto = new GetProjectDto
                    {
                        Id = bid.Id,
                        ContractorBidId = bid.ContractorBidId,
                        StartedAt = bid.StartedAt,
                        CompletedAt = bid.CompletedAt,
                        ProjectStatuses = bid.ProjectStatuses.Select(p => new ProjectStatus
                        {
                            Id = p.Id,
                            Status = p.Status,
                            UpdatedAt = p.UpdatedAt,
                            UpdatedBy = p.UpdatedBy,
                        }).ToList(),

                    };
                    return new Result<GetProjectDto>().WithValue(null).Success("پروژه پیدا شد");
                }
            }
            catch (Exception ex)
            {
                return new Result<GetProjectDto>().WithValue(null).Failure(ex.Message);
            }

        }

        public async Task<Result<GetProjectDto>> UpdateAsync(GetProjectDto projectDto , CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .Where(x => x.Id == projectDto.Id && x.IsDeleted == false)
                    .Include(x => x.ContractorBid)
                    .Include(x => x.ProjectStatuses)
                    .FirstOrDefaultAsync(cancellationToken);
                if (project is null)
                {
                    return new Result<GetProjectDto>().WithValue(null).Failure(ErrorMessages.ProjectNotFound);
                }
                else
                {
                    project.CompletedAt = projectDto.CompletedAt;
                    project.StartedAt = project.StartedAt;
                    project.IsDeleted = projectDto.IsDeleted;
                    project.DeletedBy = projectDto.DeletedBy;
                    project.DeletedAt = DateTime.Now;
                    project.UpdatedAt = DateTime.Now;
                    project.UpdatedBy = projectDto.UpdatedBy;
                    _context.Projects.Update(project);
                    await _context.SaveChangesAsync();
                    return new Result<GetProjectDto>().WithValue(projectDto).Success("پروژه آپدیت شد.");
                }

            }
            catch (Exception ex)
            {
                return new Result<GetProjectDto>().WithValue(null).Failure(ex.Message);
            }
        }
    }
}
