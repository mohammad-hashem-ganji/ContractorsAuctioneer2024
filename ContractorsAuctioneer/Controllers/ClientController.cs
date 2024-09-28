using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ContractorsAuctioneer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IBidOfContractorService _bidOfContractorService;
        private readonly IProjectService _projectService;
        private readonly IRequestService _requestService;
        public ClientController(IBidOfContractorService bidOfContractorService, IProjectService projectService, IRequestService requestService)
        {
            _bidOfContractorService = bidOfContractorService;
            _projectService = projectService;
            _requestService = requestService;
        }

        [HttpPost]
        [Route(nameof(AcceptBid))]
        public async Task<IActionResult> AcceptBid(int bidId, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _bidOfContractorService.GetByIdAsync(bidId, cancellationToken);
                if (entity.IsSuccessful && entity.Data != null)
                {
                    entity.Data.IsAccepted = true;
                    var result = await _bidOfContractorService.UpdateAsync(entity.Data, cancellationToken);
                    if (!result.IsSuccessful)
                    {
                        return BadRequest(result);
                    }
                    var addProjectDto = new AddProjectDto
                    {
                        ContractorBidId = result.Data.Id
                    };
                    var newProject = await _projectService.AddAsync(addProjectDto, cancellationToken);
                    if (!newProject.IsSuccessful)
                    {
                        return BadRequest(newProject);
                    }

                    var request = await _requestService.GetByIdAsync(result.Data.RequestId, cancellationToken);
                    if (!request.IsSuccessful)
                    {
                        return BadRequest(request);
                    }
                    request.Data.IsTenderOver = true;
                    var requestResult = await _requestService.UpdateAsync(request.Data, cancellationToken);
                    if (!requestResult.IsSuccessful)
                    {
                        return BadRequest(requestResult);
                    }
                    return NoContent();
                }
                return NotFound(entity);
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
