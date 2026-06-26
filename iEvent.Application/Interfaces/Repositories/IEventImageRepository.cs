using iEvent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IEventImageRepository
    {
        Task<List<EventImage>> GetByEventIdAsync(Guid eventId);
        Task AddRangeAsync(List<EventImage> images);
        Task DeleteAsync(EventImage image);
    }
}
