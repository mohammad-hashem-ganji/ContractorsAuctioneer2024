using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Results;
using ContractorsAuctioneer.Services;
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

        public BidOfContractorController(IBidOfContractorService bidOfContractorService, IContractorService contractorService)
        {
            _bidOfContractorService = bidOfContractorService;
            _contractorService = contractorService;
        }

        [HttpPost]
        [Route(nameof(AddBid))]
        public async Task<IActionResult> AddBid([FromBody] AddBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _bidOfContractorService.AddAsync(bidOfContractorDto, cancellationToken);
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
        [Route(nameof(GetbidById))]
        public async Task<IActionResult> GetbidById(int bidId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _bidOfContractorService.GetByIdAsync(bidId, cancellationToken);
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
        [HttpGet]
        [Route(nameof(GetAllBids))]
        public async Task<IActionResult> GetAllBids(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _bidOfContractorService.GetAllAsync(cancellationToken);
                if (result.IsSuccessful)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound(result);
                }
            }
            catch (Exception ex)
            {
                // Log 
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "خطایی در بازیابی پیشنهادات رخ داده است.",
                        Details = ex.Message
                    });
            }
        }
        [HttpPost]
        [Route(nameof(UpdateBid))]
        public async Task<IActionResult> UpdateBid([FromBody] BidOfContractorDto bidDto, CancellationToken cancellationToken)
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
        [Route(nameof(DeleteBid))]
        public async Task<IActionResult> DeleteBid(int bidId, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _bidOfContractorService.GetByIdAsync(bidId, cancellationToken);
                if (entity.IsSuccessful && entity.Data != null)
                {
                    entity.Data.IsDeleted = true;
                    var result = await _bidOfContractorService.UpdateAsync(entity.Data, cancellationToken);
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

        [HttpGet]
        [Route(nameof(GetBidsOfContractor))]
        public async Task<IActionResult> GetBidsOfContractor(int contractorId, CancellationToken cancellationToken)
        {
            try
            {
                var bidsOfContractor = await _bidOfContractorService.GetBidsOfContractorAsync(contractorId, cancellationToken);
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
