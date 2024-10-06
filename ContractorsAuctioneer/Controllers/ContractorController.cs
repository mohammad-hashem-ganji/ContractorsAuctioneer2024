using ContractorsAuctioneer.Dtos;
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

        public ContractorController(IRequestService requestService, IRequestStatusService requestStatusService)
        {
            _requestService = requestService;
            _requestStatusService = requestStatusService;
        }

        [HttpPut]
        [Route(nameof(RejectRequest))]
        public async Task<IActionResult> RejectRequest(UpdateRejectedRequestDto rejectedRequestDto, CancellationToken cancellationToken)
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
                var newStatus = new RequestStatusDto
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = rejectedRequestDto.UpdatedBy,
                    RequestId = rejectedRequestDto.RequestId,
                    Status = Entites.RequestStatusEnum.RequestApprovedByContractor
                };
                var newRequestStatus = await _requestStatusService.AddAsync(newStatus, cancellationToken);
                if (!newRequestStatus.IsSuccessful) return BadRequest(newRequestStatus);
                return Ok();
            }
            else return BadRequest("مقادیر ورودی نا معتبر است");
        }
    }
}
