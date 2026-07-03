using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IEventImageRepository
    {
        Task<EventImage?> GetByIdAsync(Guid imageId);
        Task<List<EventImage>> GetByEventIdAsync(Guid eventId);
        Task AddRangeAsync(List<EventImage> images);
        Task DeleteAsync(EventImage image);
        Task DeleteRangeAsync(List<EventImage> images);
    }
}
