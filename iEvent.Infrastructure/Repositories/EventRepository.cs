using iEvent.Application.DTOs;
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
                dbQuery = dbQuery.OrderByDescending(e => e.EventDates.Min(d => d.Date));
            }
            else 
            {
                dbQuery = dbQuery.OrderBy(e => e.EventDates.Min(d => d.Date));
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
