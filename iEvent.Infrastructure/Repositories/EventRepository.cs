using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public EventRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Event ievent)
        {
            _dbContext.Events.Add(ievent);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Event ievent)
        {
            _dbContext.Events.Remove(ievent);
            await _dbContext.SaveChangesAsync();
        }

        public Task<List<Event>> GetAllAsync()
        {
            return _dbContext.Events.AsNoTracking().ToListAsync();
        }

        public Task<Event?> GetByIdAsync(Guid id)
        {
            return _dbContext.Events.FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task UpdateAsync(Event ievent)
        {
            _dbContext.Events.Update(ievent);
            await _dbContext.SaveChangesAsync();
        }
    }
}
