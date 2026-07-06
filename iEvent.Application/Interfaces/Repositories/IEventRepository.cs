using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;
using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IEventRepository
    {
        Task<PagedResultDto<Event>> GetAllAsync(EventQueryDto query);
        Task<Event?> GetByIdAsync(Guid id);
        Task<List<EventBannerDto>> GetEventBannersAsync(int count);
        Task AddAsync(Event ievent);
        Task UpdateAsync(Event ievent);
        Task ReplaceEventChildrenAsync(Guid eventId, List<EventDate> dates, List<EventImage> images);
        Task UpdateEventDatesAsync(Guid eventId, List<EventDate> newDates);
        Task DeleteAsync(Event ievent);
        Task<List<Event>> GetEventsByVenueIdAsync(Guid venueId);
        Task<List<Event>> GetPopularEventsAsync(int count);
        Task AddEventDatesRangeAsync(List<EventDate> newDates);

    }
}
