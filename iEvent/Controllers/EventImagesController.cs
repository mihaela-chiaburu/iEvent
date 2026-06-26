using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:guid}/images")]
    public class EventImagesController : ControllerBase
    {
        private readonly IEventImageService _service;

        public EventImagesController(IEventImageService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(Guid eventId)
        {
            var images = await _service.GetByEventIdAsync(eventId);
            return Ok(images);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Upload(Guid eventId, [FromForm] List<IFormFile> files)
        {
            var urls = await _service.UploadAsync(eventId, files);
            return Ok(new { urls });
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpDelete("{imageId:guid}")]
        public async Task<IActionResult> Delete(Guid imageId)
        {
            var result = await _service.DeleteAsync(imageId);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}