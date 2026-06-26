using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Infrastructure.Repositories
{
    public class EventImageRepository : IEventImageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EventImageRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EventImage>> GetByEventIdAsync(Guid eventId)
        {
            return await _dbContext.EventImages
                .AsNoTracking()
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<EventImage> images)
        {
            _dbContext.EventImages.AddRange(images);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(EventImage image)
        {
            _dbContext.EventImages.Remove(image);
            await _dbContext.SaveChangesAsync();
        }
    }
}
