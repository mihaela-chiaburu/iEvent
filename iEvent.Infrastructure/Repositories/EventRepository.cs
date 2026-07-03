using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;
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

        public async Task<PagedResultDto<Event>> GetAllAsync(EventQueryDto query)
        {
            var dbQuery = _dbContext.Events
                .AsNoTracking()
                .Where(e => !e.IsDraft) 
                .Include(e => e.EventDates)
                    .ThenInclude(ed => ed.TimeSlots)
                .Include(e => e.Images)
                .Include(e => e.Tickets) 
                .Include(e => e.Venue)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                dbQuery = dbQuery.Where(e => e.Venue != null && EF.Functions.Like(e.Venue.City, $"%{query.City}%"));
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
            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var items = await dbQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<Event>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public Task<Event?> GetByIdAsync(Guid id)
        {
            return _dbContext.Events.Include(e => e.EventDates).ThenInclude(ed => ed.TimeSlots)
                .Include(e => e.Venue).Include(e => e.Images).Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task UpdateAsync(Event ievent)
        {
            if (_dbContext.Entry(ievent).State == EntityState.Detached)
            {
                _dbContext.Events.Update(ievent);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task ReplaceEventChildrenAsync(Guid eventId, List<EventDate> dates, List<EventImage> images)
        {
            var existingDates = await _dbContext.EventDates
                .Where(x => x.EventId == eventId)
                .ToListAsync();

            var existingImages = await _dbContext.EventImages
                .Where(x => x.EventId == eventId)
                .ToListAsync();

            _dbContext.EventDates.RemoveRange(existingDates);
            _dbContext.EventImages.RemoveRange(existingImages);

            await _dbContext.EventDates.AddRangeAsync(dates);
            await _dbContext.EventImages.AddRangeAsync(images);

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateEventDatesAsync(Guid eventId, List<EventDate> newDates)
        {
            var oldDates = await _dbContext.EventDates.Where(ed => ed.EventId == eventId).ToListAsync();
            _dbContext.EventDates.RemoveRange(oldDates);

            await _dbContext.EventDates.AddRangeAsync(newDates);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Event>> GetEventsByVenueIdAsync(Guid venueId)
        {
            return await _dbContext.Events
                .AsNoTracking()
                .Where(e => e.VenueId == venueId)
                .ToListAsync();
        }

        public async Task<List<Event>> GetPopularEventsAsync(int count)
        {
            return await _dbContext.Events
                .AsNoTracking()
                .Where(e => !e.IsDraft)
                .Include(e => e.EventDates)
                    .ThenInclude(ed => ed.TimeSlots)
                .Include(e => e.Images)
                .Include(e => e.Tickets)
                .OrderByDescending(e => _dbContext.Bookings.Count(b => b.EventId == e.EventId))
                .ThenBy(e => e.EventDates.Any() ? e.EventDates.Min(ed => ed.Date) : DateOnly.MaxValue)
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
