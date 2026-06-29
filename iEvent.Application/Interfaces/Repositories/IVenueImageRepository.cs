using iEvent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IVenueImageRepository
    {
        Task<List<VenueImage>> GetByVenueIdAsync(Guid venueId);
        Task AddRangeAsync(List<VenueImage> images);
        Task DeleteAsync(VenueImage image);
    }
}
