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
        private readonly IRejectedRequestByClientService _rejectedRequestService;
        private readonly IRequestStatusService _requestStatusService;
        private readonly IBidStatusService _bidStatusService;
        public ClientController(IBidOfContractorService bidOfContractorService, IProjectService projectService, IRequestService requestService, IRejectedRequestByClientService rejectedRequestService, IRequestStatusService requestStatusService, IBidStatusService bidStatusService)
        {
            _bidOfContractorService = bidOfContractorService;
            _projectService = projectService;
            _requestService = requestService;
            _rejectedRequestService = rejectedRequestService;
            _requestStatusService = requestStatusService;
            _bidStatusService = bidStatusService;
        }

        [HttpPost]
        [Route(nameof(AcceptBid))]
        public async Task<IActionResult> AcceptBid(UpdateBidAcceptanceDto bidDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var bid = await _bidOfContractorService.GetByIdAsync(bidDto.BidId, cancellationToken);
                if (!bid.IsSuccessful)
                {
                    return NotFound(bid);
                }
                if (bidDto.IsAccepted == true)
                {
                    bid.Data!.IsAccepted = true; // update it
                    var allBidsOfRequest = await _bidOfContractorService.GetBidsOfRequestAsync(bid.Data.RequestId, cancellationToken);
                    if (!allBidsOfRequest.IsSuccessful)
                    {
                        return NotFound(allBidsOfRequest);
                    }
                    var otherUnAcceptedBids = allBidsOfRequest.Data.Where(x => x.Id != bidDto.BidId);
                    if (!otherUnAcceptedBids.Any())
                    {
                        var un
                    }


                    var newStatus = new AddBidStatusDto
                    {
                        BidOfContractorId = bidDto.BidId,
                        Status = Entites.BidStatusEnum.BidApprovedByClient,
                        CreatedAt = DateTime.Now,
                        CreatedBy = bidDto.UpdatedBy
                    };
                    var result = await _bidStatusService.AddAsync(newStatus, cancellationToken);
                    if (!result.IsSuccessful)
                    {
                        return Problem(result.Message);
                    }
                }


                var entity = await _bidOfContractorService.GetByIdAsync(bidDto.BidId, cancellationToken);
                if (entity.IsSuccessful && entity.Data != null)
                {

                    // go to project flow
                    //var newProject = await _projectService.AddAsync(addProjectDto, cancellationToken);
                    //if (!newProject.IsSuccessful) return BadRequest(newProject);
                    //var request = await _requestService.GetByIdAsync(result.Data.RequestId, cancellationToken);
                    //if (!request.IsSuccessful || request.Data == null) return BadRequest(request);
                    //request.Data.IsTenderOver = true;
                    //var requestResult = await _requestService.UpdateAsync(request.Data, cancellationToken);
                    //if (!requestResult.IsSuccessful) return BadRequest(requestResult);
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
        [HttpPost]
        [Route(nameof(AcceptRequest))]
        public async Task<IActionResult> AcceptRequest([FromBody] UpdateRequestAcceptanceDto requestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = await _requestService.GetByIdAsync(requestDto.RequestId, cancellationToken);
            if ((!request.IsSuccessful || request.Data == null) && request.Data.IsActive == true) return NotFound(request);
            if (requestDto.IsAccepted == true)
            {
                var newStatus = new AddRequestStatusDto
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = requestDto.UpdatedBy,
                    RequestId = requestDto.RequestId,
                    Status = Entites.RequestStatusEnum.RequestApprovedByClient
                };

                var newRequestStatus = await _requestStatusService.AddAsync(newStatus, cancellationToken);
                if (!newRequestStatus.IsSuccessful) return Problem(newRequestStatus.Message);
                request.Data.ExpireAt = DateTime.Now.AddDays(7);
                var updateResult = await _requestService.UpdateAsync(request.Data, cancellationToken);
                if (!updateResult.IsSuccessful)
                {
                    return Problem(updateResult.Message);
                }

                return Ok();
            }
            else return BadRequest("مقادیر ورودی نا معتبر است");
        }

        [HttpPost]
        [Route(nameof(RejectingRequest))]
        public async Task<IActionResult> RejectingRequest([FromBody] UpdateRequestAcceptanceDto requestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = await _requestService.GetByIdAsync(requestDto.RequestId, cancellationToken);
            if (!request.IsSuccessful || request.Data == null) return NotFound(request);
            if (requestDto.IsAccepted == false)
            {
                var newStatus = new AddRequestStatusDto
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = requestDto.UpdatedBy,
                    RequestId = requestDto.RequestId,
                    Status = Entites.RequestStatusEnum.RequestRejectedByClient
                };
                var newRequestStatus = await _requestStatusService.AddAsync(newStatus, cancellationToken);
                if (!newRequestStatus.IsSuccessful) return Problem(newRequestStatus.Message);
                return Ok();
                //از اینجا به بعد باید به سامانه مهندسی ارجا داده بشه.
            }
            else return BadRequest("مقادیر ورودی نا معتبر است");
        }

        [HttpPost]
        [Route(nameof(GetBidsOfRequest))]
        public async Task<IActionResult> GetBidsOfRequest(int requestId, CancellationToken cancellationToken)
        {
            if (requestId < 0)
            {
                return BadRequest("مقدرا ورودی نا معتبر است");
            }
            var bidsOfRequest = await _bidOfContractorService.GetBidsOfRequestAsync(requestId, cancellationToken);
            if (!bidsOfRequest.IsSuccessful)
            {
                return Problem(bidsOfRequest.Message);
            }
            return Ok(bidsOfRequest);
        }
        // select (Accept one bid)




    }
}
