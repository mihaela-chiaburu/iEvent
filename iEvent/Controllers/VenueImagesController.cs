using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/venues/{venueId:guid}/images")]
    public class VenueImagesController : ControllerBase
    {
        private readonly IVenueImageService _venueImageService;

        public VenueImagesController(IVenueImageService service)
        {
            _venueImageService = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(Guid venueId)
        {
            var images = await _venueImageService.GetByVenueIdAsync(venueId);
            return Ok(images);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Upload(Guid venueId, [FromForm] List<IFormFile> files)
        {
            var urls = await _venueImageService.UploadAsync(venueId, files);
            return Ok(new { urls });
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpDelete("{imageId:guid}")]
        public async Task<IActionResult> Delete(Guid imageId)
        {
            var result = await _venueImageService.DeleteAsync(imageId);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
