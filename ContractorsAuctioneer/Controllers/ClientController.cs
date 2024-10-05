using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Services;
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
        private readonly IRejectedRequestService _rejectedRequestService;
        private readonly IRequestStatusService _requestStatusService;
        public ClientController(IBidOfContractorService bidOfContractorService, IProjectService projectService, IRequestService requestService, IRejectedRequestService rejectedRequestService, IRequestStatusService requestStatusService)
        {
            _bidOfContractorService = bidOfContractorService;
            _projectService = projectService;
            _requestService = requestService;
            _rejectedRequestService = rejectedRequestService;
            _requestStatusService = requestStatusService;
        }

        [HttpPut]
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
                    if (!result.IsSuccessful || result.Data == null) return BadRequest(result);
                    var addProjectDto = new AddProjectDto
                    {
                        ContractorBidId = result.Data.Id
                    };
                    var newProject = await _projectService.AddAsync(addProjectDto, cancellationToken);
                    if (!newProject.IsSuccessful) return BadRequest(newProject);
                    var request = await _requestService.GetByIdAsync(result.Data.RequestId, cancellationToken);
                    if (!request.IsSuccessful || request.Data == null) return BadRequest(request);
                    request.Data.IsTenderOver = true;
                    var requestResult = await _requestService.UpdateAsync(request.Data, cancellationToken);
                    if (!requestResult.IsSuccessful) return BadRequest(requestResult);
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
        [HttpPut]
        [Route(nameof(AcceptRequest))]
        public async Task<IActionResult> AcceptRequest([FromBody] UpdateRejectedRequestDto requestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var request = await _requestService.GetByIdAsync(requestDto.RequestId, cancellationToken);
                if (!request.IsSuccessful || request.Data == null) return NotFound(request);
                if (requestDto.IsAccepted == true) request.Data.IsAcceptedByClient = true;
                var updateRequest = await _requestService.UpdateAsync(request.Data, cancellationToken);
                if (!updateRequest.IsSuccessful) return BadRequest(updateRequest);
                var requestStatus = await _requestStatusService.GetRequestStatusByRequestId(requestDto.RequestId, cancellationToken);
                if (!requestStatus.IsSuccessful)
                {
                    var newStatus = new RequestStatusDto
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = requestDto.UpdatedBy,
                        RequestId = requestDto.RequestId,
                        Status = Entites.RequestStatusEnum.Approved
                    };
                    var newRequestStatus = await _requestStatusService.AddAsync(newStatus, cancellationToken);
                    if (!newRequestStatus.IsSuccessful) return BadRequest(newRequestStatus);
                    else return Ok();
                }
                else
                {
                    requestStatus.Data.UpdatedAt = DateTime.Now;
                    requestStatus.Data.UpdatedBy = requestDto.UpdatedBy;
                    requestStatus.Data.Status = Entites.RequestStatusEnum.Approved;
                    var result = await _requestStatusService.UpdateAsync(requestStatus.Data, cancellationToken);
                    if (!result.IsSuccessful) return BadRequest(result);
                    else return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
               new
               {
                   Message = "An error occurred while retrieving the requests.",
                   Details = ex.Message
               });
            }
        }

        [HttpPut]
        [Route(nameof(RejectingRequest))]
        public async Task<IActionResult> RejectingRequest([FromBody] UpdateRejectedRequestDto requestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var request = await _requestService.GetByIdAsync(requestDto.RequestId, cancellationToken);
                if (!request.IsSuccessful || request.Data == null) return NotFound(request);
                request.Data.IsAcceptedByClient = false;
                var updateRequest = await _requestService.UpdateAsync(request.Data, cancellationToken);
                if (!updateRequest.IsSuccessful) return BadRequest(updateRequest);
                var result = await _rejectedRequestService.AddAsync(requestDto, cancellationToken);
                if (!result.IsSuccessful) return BadRequest(result);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
               new
               {
                   Message = "An error occurred while retrieving the requests.",
                   Details = ex.Message
               });
            }
        }


    }
}
