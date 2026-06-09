using iEvent.Domain.Entities;
using iEvent.Domain.Enums;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync(string? city, Guid? venueId, EventCategory? category, DateTime? fromDate, DateTime? toDate);
        Task<Event?> GetByIdAsync(Guid id);
        Task AddAsync(Event ievent);
        Task UpdateAsync(Event ievent);
        Task DeleteAsync(Event ievent);
    }
}
