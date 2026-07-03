using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Repositories
{
    public class VenueImageRepository : IVenueImageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VenueImageRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<VenueImage?> GetByIdAsync(Guid imageId)
        {
            return _dbContext.VenueImages.FirstOrDefaultAsync(x => x.ImageId == imageId);
        }

        public async Task<List<VenueImage>> GetByVenueIdAsync(Guid venueId)
        {
            return await _dbContext.VenueImages
                .AsNoTracking()
                .Where(x => x.VenueId == venueId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<VenueImage> images)
        {
            _dbContext.VenueImages.AddRange(images);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(VenueImage image)
        {
            _dbContext.VenueImages.Remove(image);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<VenueImage> images)
        {
            _dbContext.VenueImages.RemoveRange(images);
            await _dbContext.SaveChangesAsync();
        }
    }
}
