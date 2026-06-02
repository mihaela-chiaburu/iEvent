using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;
using iEvent.Domain.Enums;

namespace iEvent.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;

        public BookingService(IBookingRepository bookingRepository, ITicketTypeRepository ticketTypeRepository)
        {
            _bookingRepository = bookingRepository;
            _ticketTypeRepository = ticketTypeRepository;
        }

        public async Task<List<BookingRespDto>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Select(MapToRespDto).ToList();
        }

        public async Task<BookingRespDto?> GetByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking == null ? null : MapToRespDto(booking);
        }

        public async Task<BookingRespDto?> CreateAsync(BookingCreateDto dto)
        {
            var ticketTypeIds = dto.Tickets.Select(t => t.TicketTypeId).ToList();
            var ticketTypes = await _ticketTypeRepository.GetByIdsAsync(ticketTypeIds);

            if (ticketTypes.Count != ticketTypeIds.Count)
            {
                return null;
            }

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                EventId = dto.EventId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Pending
            };

            foreach (var ticket in dto.Tickets)
            {
                var ticketType = ticketTypes.First(t => t.TicketTypeId == ticket.TicketTypeId);
                var bookingTicket = new BookingTicket
                {
                    BookingTicketId = Guid.NewGuid(),
                    BookingId = booking.BookingId,
                    TicketTypeId = ticketType.TicketTypeId,
                    Quantity = ticket.Quantity,
                    UnitPrice = ticketType.Price
                };

                booking.BookingTickets.Add(bookingTicket);
            }

            booking.TotalPrice = booking.BookingTickets.Sum(bt => bt.UnitPrice * bt.Quantity);

            await _bookingRepository.AddAsync(booking);
            return MapToRespDto(booking);
        }

        public async Task<bool> UpdateAsync(Guid id, BookingUpdateDto dto)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return false;
            }

            booking.Status = dto.Status;
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return false;
            }

            await _bookingRepository.DeleteAsync(booking);
            return true;
        }

        private static BookingRespDto MapToRespDto(Booking booking)
        {
            return new BookingRespDto
            {
                BookingId = booking.BookingId,
                CustomerId = booking.CustomerId,
                EventId = booking.EventId,
                BookingDate = booking.BookingDate,
                Status = booking.Status,
                TotalPrice = (double)booking.TotalPrice,
                Tickets = booking.BookingTickets.Select(bt => new BookingTicketRespDto
                {
                    BookingTicketId = bt.BookingTicketId,
                    TicketTypeId = bt.TicketTypeId,
                    Quantity = bt.Quantity,
                    UnitPrice = (double)bt.UnitPrice
                }).ToList()
            };
        }
    }
}
