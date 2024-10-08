﻿using ContractorsAuctioneer.Dtos;
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
        private readonly IBidStatusService _bidStatusService;
        private readonly IContractorService _contractorService;
        private readonly IBidOfContractorService _bidOfContractorService;
        private readonly IProjectService _projectService;
        public ContractorController(IRequestService requestService, IRequestStatusService requestStatusService, IContractorService contractorService, IBidOfContractorService bidOfContractorService, IBidStatusService bidStatusService, IProjectService projectService)
        {
            _requestService = requestService;
            _requestStatusService = requestStatusService;
            _contractorService = contractorService;
            _bidOfContractorService = bidOfContractorService;
            _bidStatusService = bidStatusService;
            _projectService = projectService;
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
        [HttpGet]
        [Route(nameof(ShowBidsAcceptedByClient))]
        public async Task<IActionResult> ShowBidsAcceptedByClient(int contractorId, CancellationToken cancellationToken)
        {
            var acceptedBids = await _bidOfContractorService.GetBidsAcceptedByClient(contractorId, cancellationToken);
            if (!acceptedBids.IsSuccessful)
            {
                return NotFound(acceptedBids);
            }
            return Ok(acceptedBids);
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
                    Status = Entites.BidStatusEnum.BidApprovedByContractor,
                    CreatedAt = DateTime.Now,
                    CreatedBy = bidDto.UpdatedBy
                };
                var newBidStatus = await _bidStatusService.AddAsync(newStatus, cancellationToken);
                if (!newBidStatus.IsSuccessful)
                {
                    return Problem(newBidStatus.Message);
                }
                else
                {
                    var newProject = new AddProjectDto
                    {
                        ContractorBidId = bidDto.BidId
                    };
                    var projectResult = await _projectService.AddAsync(newProject, cancellationToken);
                    if (!projectResult.IsSuccessful)
                    {
                        return Problem(projectResult.Message);
                    }
                    return Ok(projectResult);
                }
            }

            return Problem(ErrorMessages.ErrorWhileAcceptingBid);
        }
        [HttpPost]
        [Route(nameof(RejectBidByContractor))]
        public async Task<IActionResult> RejectBidByContractor(UpdateBidAcceptanceDto bidDto, CancellationToken cancellationToken)
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
            if (bidDto.IsAccepted == false)
            {

                var newStatus = new AddBidStatusDto
                {
                    BidOfContractorId = bid.Data.Id,
                    Status = Entites.BidStatusEnum.BidRejectedByContractor,
                    CreatedAt = DateTime.Now,
                    CreatedBy = bidDto.UpdatedBy
                };
                var newBidStatus = await _bidStatusService.AddAsync(newStatus, cancellationToken);
                if (!newBidStatus.IsSuccessful)
                {
                    return Problem(newBidStatus.Message);
                }
                var requestDto = await _requestService.GetByIdAsync(bid.Data.RequestId, cancellationToken);
                if (!requestDto.IsSuccessful)
                {
                    return Problem(requestDto.Message);
                }
                requestDto.Data.ExpireAt = DateTime.Now.AddDays(7);
                var requestResult = await _requestService.UpdateAsync(requestDto.Data, cancellationToken);
                if (!requestResult.IsSuccessful)
                {
                    return Problem(requestResult.Message);
                }
                return Ok(newBidStatus);

            }
            return BadRequest(ErrorMessages.AnErrorWhileUpdatingStatus);
        }
    }
}