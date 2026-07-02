using iEvent.Application.DTOs.Venue;
using Microsoft.AspNetCore.Http;

namespace iEvent.Application.Interfaces.Services
{
    public interface IVenueImageService
    {
        Task<List<string>> UploadAsync(Guid venueId, List<IFormFile> files);
        Task<List<VenueImageRespDto>> GetByVenueIdAsync(Guid venueId);
        Task<bool> DeleteAsync(Guid imageId);
    }
}
