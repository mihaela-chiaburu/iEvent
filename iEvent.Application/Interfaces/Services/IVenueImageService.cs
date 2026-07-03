using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Venue;

namespace iEvent.Application.Interfaces.Services
{
    public interface IVenueImageService
    {
        Task<List<VenueImageRespDto>> UploadAsync(Guid venueId, List<FileUploadDto> files);
        Task<List<VenueImageRespDto>> GetByVenueIdAsync(Guid venueId);
        Task<bool> DeleteAsync(Guid imageId);
        Task DeleteByVenueIdAsync(Guid venueId);
    }
}
