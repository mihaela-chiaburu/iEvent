using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:guid}/images")]
    public class EventImagesController : ControllerBase
    {
        private readonly IEventImageService _EventImageService;

        public EventImagesController(IEventImageService EventImageService)
        {
            _EventImageService = EventImageService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(Guid eventId)
        {
            var images = await _EventImageService.GetByEventIdAsync(eventId);
            return Ok(images);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<List<EventImageRespDto>>> Upload(Guid eventId, [FromForm] List<IFormFile> files)
        {
            var uploads = new List<FileUploadDto>();

            foreach (var file in files)
            {
                await using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                uploads.Add(new FileUploadDto
                {
                    Content = stream.ToArray(),
                    FileName = file.FileName
                });
            }

            var images = await _EventImageService.UploadAsync(eventId, uploads);
            return Ok(images);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpDelete("{imageId:guid}")]
        public async Task<IActionResult> Delete(Guid imageId)
        {
            var result = await _EventImageService.DeleteAsync(imageId);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}