using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Services;
using ContractorsAuctioneer.Utilities.Constants;
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
        private readonly IRequestStatusService _requestStatusService;
        private readonly IBidStatusService _bidStatusService;
        public ClientController(IBidOfContractorService bidOfContractorService, IProjectService projectService, IRequestService requestService, IRequestStatusService requestStatusService, IBidStatusService bidStatusService)
        {
            _bidOfContractorService = bidOfContractorService;
            _projectService = projectService;
            _requestService = requestService;
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
            var bid = await _bidOfContractorService.GetByIdAsync(bidDto.BidId, cancellationToken);
            if (!bid.IsSuccessful)
            {
                return NotFound(bid);
            }
            if (bidDto.IsAccepted == true)
            {

                var newStatus = new AddBidStatusDto
                {
                    BidOfContractorId = bid.Data.Id,
                    Status = Entites.BidStatusEnum.BidApprovedByClient,
                    CreatedAt = DateTime.Now,
                };
                var newBidStatus = await _bidStatusService.AddAsync(newStatus, cancellationToken);
                if (!newBidStatus.IsSuccessful)
                {
                    return Problem(newBidStatus.Message);
                }
                bid.Data.ExpireAt = DateTime.Now.AddDays(2);
                var updatecontract = new UpdateBidOfContractorDto
                { 
                    ExpireAt = bid.Data.ExpireAt,
                    Id = bid.Data.Id,                   
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = bid.Data.UpdatedBy
                };
                var updatedBid = await _bidOfContractorService.UpdateAsync(updatecontract, cancellationToken);
                if (!updatedBid.IsSuccessful)
                {
                    return Problem(updatedBid.Message);
                }
                return Ok(updatedBid);
            }
            return BadRequest();


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
                    //CreatedBy = requestDto.
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
                    //CreatedBy = requestDto.,
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
