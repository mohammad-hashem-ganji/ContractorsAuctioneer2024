using ContractorsAuctioneer.Dtos;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsAuctioneer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileAttachmentController : ControllerBase
    {
        private readonly IFileAttachmentService _fileAttachmentService;

        public FileAttachmentController(IFileAttachmentService fileAttachmentService)
        {
            _fileAttachmentService = fileAttachmentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto model, CancellationToken cancellationToken)
        {
            var result = await _fileAttachmentService.AddAsync(model, cancellationToken);

            if (result.IsSuccessful)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
