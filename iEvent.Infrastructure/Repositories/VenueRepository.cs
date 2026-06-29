using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VenueRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Venue>> GetAllAsync()
        {
            return _dbContext.Venues.AsNoTracking()
                .Include(v => v.Events).Include(v => v.Facilities).Include(v => v.Images).ToListAsync();
        }

        public Task<Venue?> GetByIdAsync(Guid id)
        {
            return _dbContext.Venues.Include(v => v.Facilities).Include(v => v.Events).Include(v => v.Images)
                            .FirstOrDefaultAsync(v => v.VenueId == id);
        }

        public async Task<List<Venue>> GetPopularAsync(int take = 10)
        {
            return await _dbContext.Venues
                .AsNoTracking()
                .Include(v => v.Facilities).Include(v => v.Events).Include(v => v.Images)
                .OrderByDescending(v => v.Events.Count(e => !e.IsDraft))
                .Take(take)
                .ToListAsync();
        }

        public async Task AddAsync(Venue venue)
        {
            _dbContext.Venues.Add(venue);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Venue venue)
        {
            _dbContext.Venues.Update(venue);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Venue venue)
        {
            _dbContext.Venues.Remove(venue);
            await _dbContext.SaveChangesAsync();
        }
    }
}
