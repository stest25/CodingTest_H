using Microsoft.AspNetCore.Mvc;
using VideoMediaApp.Services;

namespace VideoMediaApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IMediaFileService _mediaFileService;

        public UploadController(IMediaFileService mediaFileService)
        {
            _mediaFileService = mediaFileService;
        }

        [HttpPost]
        [RequestSizeLimit(209715200)] // 200MB
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (!files.Any())
            {
                return BadRequest("No files selected.");
            }

            var errors = new List<string>();

            foreach (var file in files)
            {
                var result = await _mediaFileService.UploadFileAsync(file);

                if (!result.Success)
                {
                    errors.Add($"{file.FileName}: {result.ErrorMessage}");
                }
            }

            if (errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = "Files uploaded successfully." });
        }
    }
}
