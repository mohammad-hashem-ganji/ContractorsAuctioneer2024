using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsAuctioneer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidOfContractorController : ControllerBase
    {
        private readonly IBidOfContractorService _bidOfContractorService;
        private readonly IContractorService _contractorService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BidOfContractorController(IBidOfContractorService bidOfContractorService, IContractorService contractorService, IHttpContextAccessor httpContextAccessor)
        {
            _bidOfContractorService = bidOfContractorService;
            _contractorService = contractorService;
            _httpContextAccessor = httpContextAccessor;
        }
        [Authorize(Roles= "Contractor")]
        [HttpPost]
        [Route(nameof(AddBid))]
        public async Task<IActionResult> AddBid([FromBody] AddBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var addResult = await _bidOfContractorService.AddAsync(bidOfContractorDto, cancellationToken);
            if (addResult.IsSuccessful)
            {
                return Ok(addResult);
            }
            return StatusCode(500, addResult);
        }
        [HttpGet]
        [Route(nameof(GetbidById))]
        public async Task<IActionResult> GetbidById(int bidId, CancellationToken cancellationToken)
        {
            if (bidId <= 0)
            {
                return BadRequest("ورودی نامعتبر");
            }
            var result = await _bidOfContractorService.GetByIdAsync(bidId, cancellationToken);
            if (result.IsSuccessful)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
        [Authorize(Roles = "Contractor")]
        [HttpPost]
        [Route(nameof(UpdateBid))]
        public async Task<IActionResult> UpdateBid([FromBody] UpdateBidOfContractorDto bidDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
             
                var result = await _bidOfContractorService.UpdateAsync(bidDto, cancellationToken);
                if (result.IsSuccessful)
                {
                    return NoContent();
                }
                return NotFound(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "خطایی در بازیابی پیشنهادات رخ داده است.",
                    });
            }
        }
        [Authorize(Roles = "Contractor")]
        [HttpPost]
        [Route(nameof(DeleteBid))]
        public async Task<IActionResult> DeleteBid(int bidId, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _bidOfContractorService.GetByIdAsync(bidId, cancellationToken);
                if (entity.IsSuccessful && entity.Data != null)
                {
                    entity.Data.IsDeleted = true;
                    var updatecontract = new UpdateBidOfContractorDto
                    {
                        Id = entity.Data.Id,
                        IsDeleted = true,                      
                        UpdatedAt = DateTime.Now,
                    };
                    var result = await _bidOfContractorService.UpdateAsync(updatecontract, cancellationToken);
                    return NoContent();
                }
                return NotFound(entity);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "خطایی در بازیابی پیشنهادات رخ داده است.",
                    });
            }

        }
        [Authorize(Roles = "Contractor")]
        [HttpGet]
        [Route(nameof(GetBidsOfContractor))]
        public async Task<IActionResult> GetBidsOfContractor(CancellationToken cancellationToken)
        {
            var bidsOfContractor = await _bidOfContractorService.GetBidsOfContractorAsync(cancellationToken);
            if (bidsOfContractor.IsSuccessful)
            {
                //var bids = bidsOfContractor.Data;
                if (bidsOfContractor.Data.Any())
                {
                    return Ok(bidsOfContractor);
                }
                return NotFound(bidsOfContractor);
            }
            return NotFound(bidsOfContractor);



        }



    }
}
