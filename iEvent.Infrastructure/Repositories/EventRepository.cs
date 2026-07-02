using iEvent.Application.DTOs;
using iEvent.Application.DTOs.Event;
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

        public async Task<PagedResult<Event>> GetAllAsync(EventQueryDto query)
        {
            var dbQuery = _dbContext.Events
                .Where(e => !e.IsDraft) 
                .Include(e => e.EventDates)
                    .ThenInclude(ed => ed.TimeSlots)
                .Include(e => e.Images)
                .Include(e => e.Tickets) 
                .Include(e => e.Venue)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                dbQuery = dbQuery.Where(e => e.Venue != null && e.Venue.City.Contains(query.City));
            }
            if (query.VenueId.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.VenueId == query.VenueId);
            }
            if (query.Category.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.Category == query.Category);
            }
            if (query.FromDate.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.EventDates.Any(d => d.Date >= query.FromDate));
            }
            if (query.ToDate.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.EventDates.Any(d => d.Date <= query.ToDate));
            }

            if (query.MinPrice.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.Tickets.Any() && e.Tickets.Min(t => t.Price) >= query.MinPrice.Value);
            }
            if (query.MaxPrice.HasValue)
            {
                dbQuery = dbQuery.Where(e => e.Tickets.Any() && e.Tickets.Min(t => t.Price) <= query.MaxPrice.Value);
            }

            if (query.SortBy == "date_desc")
            {
                dbQuery = dbQuery.OrderByDescending(e => e.EventDates.Any() ? e.EventDates.Min(d => d.Date) : DateOnly.MaxValue);
            }
            else
            {
                dbQuery = dbQuery.OrderBy(e => e.EventDates.Any() ? e.EventDates.Min(d => d.Date) : DateOnly.MinValue);
            }

            var totalCount = await dbQuery.CountAsync();

            var items = await dbQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Event>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public Task<Event?> GetByIdAsync(Guid id)
        {
            return _dbContext.Events.Include(e => e.EventDates).ThenInclude(ed => ed.TimeSlots)
                .Include(e => e.Venue).Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task UpdateAsync(Event ievent)
        {
            //_dbContext.Events.Update(ievent);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateEventDatesAsync(Guid eventId, List<EventDate> newDates)
        {
            var oldTimeSlots = _dbContext.EventTimeSlots
                .Where(ts => _dbContext.EventDates.Where(ed => ed.EventId == eventId).Select(ed => ed.EventDateId).Contains(ts.EventDateId));
            _dbContext.EventTimeSlots.RemoveRange(oldTimeSlots);

            var oldDates = _dbContext.EventDates.Where(ed => ed.EventId == eventId);
            _dbContext.EventDates.RemoveRange(oldDates);

            await _dbContext.SaveChangesAsync();

            await _dbContext.EventDates.AddRangeAsync(newDates);
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

        public async Task AddEventDatesRangeAsync(List<EventDate> newDates)
        {
            await _dbContext.EventDates.AddRangeAsync(newDates);
            await _dbContext.SaveChangesAsync();
        }
    }
}
