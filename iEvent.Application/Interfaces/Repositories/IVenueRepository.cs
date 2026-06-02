using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IVenueRepository
    {
        Task<List<Venue>> GetAllAsync();
        Task<Venue?> GetByIdAsync(Guid id);
        Task AddAsync(Venue venue);
        Task UpdateAsync(Venue venue);
        Task DeleteAsync(Venue venue);
    }
}
