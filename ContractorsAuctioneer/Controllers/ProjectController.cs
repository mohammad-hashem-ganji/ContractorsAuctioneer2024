using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace ContractorsAuctioneer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        [HttpPost]
        [Route(nameof(AddProject))]
        public async Task<IActionResult> AddProject([FromBody] AddProjectDto projectDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _projectService.AddAsync(projectDto, cancellationToken);
                if (result.IsSuccessful)
                {
                    return Ok(result);
                }
                return StatusCode(500, result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route(nameof(GetProjectById))]
        public async Task<IActionResult> GetProjectById(int projectId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _projectService.GetByIdAsync(projectId, cancellationToken);
                if (result.IsSuccessful)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                // Log 
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An error occurred while retrieving the bid.",
                        Details = ex.Message
                    });
            }
        }

        [HttpPost]
        [Route(nameof(GetProjectOfRequest))]
        public async Task<IActionResult> GetProjectOfRequest(int requestId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _projectService.GetByIdAsync(requestId, cancellationToken);
                if (result.IsSuccessful)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                // Log 
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An error occurred while retrieving the bid.",
                        Details = ex.Message
                    });
            }
        }

        [HttpPost]
        [Route(nameof(UpdateProject))]
        public async Task<IActionResult> UpdateProject([FromBody] GetProjectDto projectDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _projectService.UpdateAsync(projectDto, cancellationToken);
                if (result.IsSuccessful)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "خطایی در بازیابی پیشنهادات رخ داده است.",
                        Details = ex.Message
                    });
            }
        }



    }
}
