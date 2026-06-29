using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Domain.Enums;
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

        public async Task<List<Event>> GetAllAsync(string? city, Guid? venueId, EventCategory? category,
            DateOnly? fromDate, DateOnly? toDate)
        {
            var query = _dbContext.Events
                .AsNoTracking()
                .Include(e => e.Venue)
                .Include(e => e.Images)
                .Include(e => e.EventDates).ThenInclude(ed => ed.TimeSlots)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(e =>
                    e.Venue != null &&
                    e.Venue.City.ToLower().Contains(city.ToLower()));
            }

            if (venueId.HasValue)
            {
                query = query.Where(e => e.VenueId == venueId.Value);
            }

            if (category.HasValue)
            {
                query = query.Where(e => e.Category == category.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(e => e.EventDates.Any(ed => ed.Date >= fromDate.Value));
            }

            if (toDate.HasValue)
            {
                query = query.Where(e => e.EventDates.Any(ed => ed.Date <= toDate.Value));
            }

            return await query
                .OrderBy(e => e.EventDates.OrderBy(ed => ed.Date).Select(ed => ed.Date).FirstOrDefault())
                .ToListAsync();
        }

        public Task<Event?> GetByIdAsync(Guid id)
        {
            return _dbContext.Events.Include(e => e.Venue).Include(e => e.Images)
                    .Include(e => e.EventDates).ThenInclude(ed => ed.TimeSlots) 
                    .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task UpdateAsync(Event ievent)
        {
            _dbContext.Events.Update(ievent);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Event>> GetEventsByVenueIdAsync(Guid venueId)
        {
            return await _dbContext.Events
                .Where(e => e.VenueId == venueId)
                .ToListAsync();
        }

        public async Task<List<Event>> GetPopularEventsAsync(int count)
        {
            return await _dbContext.Events
                .Where(e => !e.IsDraft)
                .Include(e => e.EventDates)
                    .ThenInclude(ed => ed.TimeSlots)
                .Include(e => e.Images)
                .Take(count)
                .ToListAsync();
        }
    }
}
