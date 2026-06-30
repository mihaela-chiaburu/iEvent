using iEvent.Application.DTOs;
using iEvent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task<(List<Booking> Items, int TotalCount)> GetPagedAsync(BookingFilterDto filter);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
        Task<List<Booking>> GetByCustomerIdAsync(Guid customerId);
        Task<Booking?> GetByCodeAsync(string code);
    }
}
