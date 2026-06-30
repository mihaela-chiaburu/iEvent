using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BookingRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Booking>> GetAllAsync()
        {
            return _dbContext.Bookings
                .Include(b => b.BookingTickets)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Booking?> GetByIdAsync(Guid id)
        {
            return _dbContext.Bookings
                .Include(b => b.BookingTickets)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task AddAsync(Booking booking)
        {
            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _dbContext.ChangeTracker.Clear();

            _dbContext.Bookings.Attach(booking);
            _dbContext.Entry(booking).State = EntityState.Modified;

            foreach (var ticket in booking.BookingTickets)
            {
                var existsInDb = _dbContext.BookingTickets
                    .Any(bt => bt.BookingTicketId == ticket.BookingTicketId);

                if (!existsInDb)
                {
                    _dbContext.Entry(ticket).State = EntityState.Added;
                }
                else
                {
                    _dbContext.Entry(ticket).State = EntityState.Modified;
                }
            }

            var modifiedTicketTypes = _dbContext.ChangeTracker
                .Entries<TicketType>()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Detached);

            foreach (var entry in modifiedTicketTypes)
            {
                if (entry.State == EntityState.Detached)
                {
                    _dbContext.TicketTypes.Attach(entry.Entity);
                    entry.State = EntityState.Modified;
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Booking booking)
        {
            _dbContext.Bookings.Remove(booking);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _dbContext.Bookings
                .Include(b => b.BookingTickets)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetByCodeAsync(string code)
        {
            return await _dbContext.Bookings
                .Include(b => b.BookingTickets)
                .FirstOrDefaultAsync(b => b.BookingCode == code);
        }
    }
}
