using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
        Task<List<Booking>> GetByCustomerIdAsync(Guid customerId);
        Task<Booking?> GetByCodeAsync(string code);
    }
}
