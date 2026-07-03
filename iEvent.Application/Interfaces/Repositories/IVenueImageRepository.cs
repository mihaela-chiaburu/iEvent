using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IVenueImageRepository
    {
        Task<VenueImage?> GetByIdAsync(Guid imageId);
        Task<List<VenueImage>> GetByVenueIdAsync(Guid venueId);
        Task AddRangeAsync(List<VenueImage> images);
        Task DeleteAsync(VenueImage image);
        Task DeleteRangeAsync(List<VenueImage> images);
    }
}
