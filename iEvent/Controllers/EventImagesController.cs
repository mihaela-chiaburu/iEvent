using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventImagesController : ControllerBase
    {
        private readonly IEventImageService _eventImageService;

        public EventImagesController(IEventImageService eventImageService)
        {
            _eventImageService = eventImageService;
        }

        [AllowAnonymous]
        [HttpGet("{eventId:guid}/images")]
        public async Task<IActionResult> Get(Guid eventId)
        {
            var images = await _eventImageService.GetByEventIdAsync(eventId);
            return Ok(images);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost("{eventId:guid}/images")]
        public async Task<ActionResult<List<EventImageRespDto>>> Upload(Guid eventId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("At least one file is required.");

            var uploads = new List<FileUploadDto>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                    continue;

                await using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                uploads.Add(new FileUploadDto
                {
                    Content = stream.ToArray(),
                    FileName = file.FileName
                });
            }

            var images = await _eventImageService.UploadAsync(eventId, uploads);
            return Ok(images);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost("{eventId:guid}/banner")]
        public async Task<ActionResult<EventImageRespDto>> PostBanner(Guid eventId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Banner file is required.");

            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            var dto = new FileUploadDto
            {
                Content = stream.ToArray(),
                FileName = file.FileName
            };

            var result = await _eventImageService.UploadBannerAsync(eventId, dto);
            return Ok(result);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpDelete("images/{imageId:guid}")]
        public async Task<IActionResult> Delete(Guid imageId)
        {
            var result = await _eventImageService.DeleteAsync(imageId);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}