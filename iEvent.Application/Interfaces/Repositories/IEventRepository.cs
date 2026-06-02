using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync(string? city);
        Task<Event?> GetByIdAsync(Guid id);
        Task AddAsync(Event ievent);
        Task UpdateAsync(Event ievent);
        Task DeleteAsync(Event ievent);
    }
}
