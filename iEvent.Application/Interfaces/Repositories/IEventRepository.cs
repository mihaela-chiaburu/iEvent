using iEvent.Application.DTOs;
using iEvent.Application.DTOs.Event;
using iEvent.Domain.Entities;
using iEvent.Domain.Enums;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IEventRepository
    {
        Task<PagedResult<Event>> GetAllAsync(EventQueryDto query);
        Task<Event?> GetByIdAsync(Guid id);
        Task AddAsync(Event ievent);
        Task UpdateAsync(Event ievent);
        Task UpdateEventDatesAsync(Guid eventId, List<EventDate> newDates);
        Task DeleteAsync(Event ievent);
        Task<List<Event>> GetEventsByVenueIdAsync(Guid venueId);
        Task<List<Event>> GetPopularEventsAsync(int count);
        Task AddEventDatesRangeAsync(List<EventDate> newDates);

    }
}
