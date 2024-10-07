using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Services;
using ContractorsAuctioneer.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsAuctioneer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IRequestStatusService _requestStatusService;
        private readonly IContractorService _contractorService;
        private readonly IBidOfContractorService _bidOfContractorService;
        public ContractorController(IRequestService requestService, IRequestStatusService requestStatusService, IContractorService contractorService, IBidOfContractorService bidOfContractorService)
        {
            _requestService = requestService;
            _requestStatusService = requestStatusService;
            _contractorService = contractorService;
            _bidOfContractorService = bidOfContractorService;
        }

        [HttpPut]
        [Route(nameof(RejectRequest))]
        public async Task<IActionResult> RejectRequest(UpdateRequestAcceptanceDto rejectedRequestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var request = await _requestService.GetByIdAsync(rejectedRequestDto.RequestId, cancellationToken);
            if (!request.IsSuccessful)
            {
                return NotFound(request);
            }
            if (rejectedRequestDto.IsAccepted == false)
            {
                var newStatus = new AddRequestStatusDto
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = rejectedRequestDto.UpdatedBy,
                    RequestId = rejectedRequestDto.RequestId,
                    Status = Entites.RequestStatusEnum.RequestRejectedByContractor
                };
                var newRequestStatus = await _requestStatusService.AddAsync(newStatus, cancellationToken);
                if (!newRequestStatus.IsSuccessful) return BadRequest(newRequestStatus);
                return Ok();
            }
            else return BadRequest("مقادیر ورودی نا معتبر است");
        }


        [HttpGet]
        [Route(nameof(ShowRequestsToContractor))]
        public async Task<IActionResult> ShowRequestsToContractor(int contractorId, CancellationToken cancellationToken)
        {
            if (contractorId <= 0)
            {
                return BadRequest(contractorId);
            }
            var requests = await _requestService.GetRequestsforContractor(contractorId, cancellationToken);
            if (!requests.IsSuccessful)
            {
                return Problem(requests.Message);
            }
            return Ok(requests);
        }

        public async Task<IActionResult> ShowBidsAcceptedByClient(int contractorId, CancellationToken cancellationToken)
        {
            var acceptedBids = await _bidOfContractorService.GetBidsAcceptedByClient(contractorId, cancellationToken);
            if (!acceptedBids.IsSuccessful)
            {
                return NotFound(acceptedBids);
            }
            return Ok(acceptedBids);
        }


    }
}
