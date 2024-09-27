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
        [Route(nameof(AddBidAsync))]
        public async Task<IActionResult> AddBidAsync([FromBody] AddBidOfContractorDto bidOfContractorDto, CancellationToken cancellationToken)
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
        [Route(nameof(GetbidByIdAsync))]
        public async Task<IActionResult> GetbidByIdAsync(int bidId, CancellationToken cancellationToken)
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
        [Route(nameof(GetAllBidsAsync))]
        public async Task<IActionResult> GetAllBidsAsync(CancellationToken cancellationToken)
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
        [Route(nameof(UpdateBidAsync))]
        public async Task<IActionResult> UpdateBidAsync([FromBody] BidOfContractorDto bidDto, CancellationToken cancellationToken)
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
                    return Ok(result);
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
        [Route(nameof(DeleteBidAsync))]
        public async Task<IActionResult> DeleteBidAsync(int bidId, CancellationToken cancellationToken)
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
                return NotFound(bidId);
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
        [Route(nameof(GetBidsOfContractorAsync))]
        public async Task<IActionResult> GetBidsOfContractorAsync(int contractorId, CancellationToken cancellationToken)
        {
            try
            {
                var contracorDto = await _contractorService.GetByIdAsync(contractorId, cancellationToken);
                if (contracorDto.IsSuccessful)
                {
                    var bids = contracorDto.Data.BidOfContractors.ToList();
                    if (bids.Any())
                    {
                        return Ok(bids);
                    }
                    return NotFound("پیشنهادی یافت نشد .");
                }
                return BadRequest("دریافت اطلاعات پیمانکار با شکست مواجه شد");
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
