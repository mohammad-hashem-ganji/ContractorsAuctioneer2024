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
        private readonly IContractorService _contractorService;
        private readonly IRequestService _requestService;
        private readonly IRequestRejecteByContractorService _requestRejecteByContractorService;
        public ContractorController(IContractorService contractorService, IRequestService requestService)
        {
            _contractorService = contractorService;
            _requestService = requestService;
        }

        [HttpPut]
        [Route(nameof(RejectRequest))]
        public async Task<IActionResult> RejectRequest(AddRejectedRequestDto rejectedRequestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resalt = await _requestRejecteByContractorService.AddAsync(rejectedRequestDto, cancellationToken);
            if (!resalt.IsSuccessful)
            {
                return BadRequest(resalt);
            }
            return BadRequest(resalt);
        }
    }
}
