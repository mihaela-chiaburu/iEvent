using iEvent.Application.DTOs.Booking;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
                .AsNoTracking()
                .Include(b => b.BookingTickets)
                .ToListAsync();
        }

        public Task<Booking?> GetByIdAsync(Guid id)
        {
            return _dbContext.Bookings
                .Include(b => b.BookingTickets)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<(List<Booking> Items, int TotalCount)> GetPagedAsync(BookingFilterDto filter)
        {
            var query = _dbContext.Bookings
                .AsNoTracking()
                .Include(b => b.BookingTickets)
                .Include(b => b.Customer)
                .AsQueryable();

            if (filter.Status.HasValue)
            {
                query = query.Where(b => b.Status == filter.Status.Value);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(b => b.BookingDate >= filter.DateFrom.Value);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(b => b.BookingDate <= filter.DateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                query = query.Where(b =>
                    EF.Functions.Like(b.BookingCode.ToLower(), $"%{searchLower}%") ||
                    EF.Functions.Like(b.Customer!.Name.ToLower(), $"%{searchLower}%") ||
                    EF.Functions.Like(b.Customer.Email.ToLower(), $"%{searchLower}%"));
            }

            var totalCount = await query.CountAsync();

            int page = filter.Page < 1 ? 1 : filter.Page;
            int pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(Booking booking)
        {
            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddBookingTicketAsync(BookingTicket bookingTicket)
        {
            _dbContext.BookingTickets.Add(bookingTicket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
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
                .AsNoTracking()
                .Include(b => b.BookingTickets)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetByCodeAsync(string code)
        {
            return await _dbContext.Bookings
                .AsNoTracking()
                .Include(b => b.BookingTickets)
                .FirstOrDefaultAsync(b => b.BookingCode == code);
        }
    }
}
