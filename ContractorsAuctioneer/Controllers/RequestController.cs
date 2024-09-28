﻿using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsAuctioneer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        [Route(nameof(AddRequestAsync))]
        public async Task<IActionResult> AddRequestAsync([FromBody] AddRequestDto requestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _requestService.AddAsync(requestDto, cancellationToken);
                if (result)
                {
                    return Ok();
                }
                    return StatusCode(500, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route(nameof(GetRequestByIdAsync))]
        public async Task<IActionResult> GetRequestByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _requestService.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccessful)
                {
                    return Ok(result);
                }
                return NotFound(result);        
            }
            catch (Exception ex)
            {
                // Log 
                return StatusCode(500,ex.Message);
            }
        }
        //[HttpGet]
        //[Route(nameof(GetRequestOfClient))]

        [HttpGet]
        [Route(nameof(GetAllRequestsAsync))]
        public async Task<IActionResult> GetAllRequestsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _requestService.GetAllAsync(cancellationToken);
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
                        Message = "An error occurred while retrieving the requests.",
                        Details = ex.Message
                    });
            }
        }

    }
}
