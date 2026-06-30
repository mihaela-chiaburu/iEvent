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
        private readonly ICustomerRepository _customerRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IEventRepository _eventRepository;

        public BookingService(IBookingRepository bookingRepository, ITicketTypeRepository ticketTypeRepository,
            ICustomerRepository customerRepository, IEventRepository eventRepository)
        {
            _bookingRepository = bookingRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _customerRepository = customerRepository;
            _eventRepository = eventRepository;
        }

        public async Task<List<BookingRespDto>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            foreach (var booking in bookings)
            {
                await ExpireIfNeededAsync(booking);
            }
            return bookings.Select(MapToRespDto).ToList();
        }

        public async Task<BookingRespDto?> GetByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return null;
            await ExpireIfNeededAsync(booking);

            return MapToRespDto(booking);
        }

        public async Task<BookingRespDto?> CreateAsync(BookingCreateDto dto, string identityUserId)
        {
            if (dto.Tickets == null || !dto.Tickets.Any())
            {
                return null;
            }

            if (dto.Tickets.Any(t => t.Quantity <= 0))
            {
                return null;
            }

            var ievent = await _eventRepository.GetByIdAsync(dto.EventId);
            if (ievent == null)
            {
                return null; 
            }

            var timeSlotExistsAndBelongsToEvent = ievent.EventDates
                .SelectMany(ed => ed.TimeSlots)
                .Any(ts => ts.TimeSlotId == dto.BookingTimeSlotId);

            if (!timeSlotExistsAndBelongsToEvent)
            {
                return null; 
            }

            var ticketTypeIds = dto.Tickets
                .Select(t => t.TicketTypeId)
                .Distinct()
                .ToList();

            var ticketTypes = await _ticketTypeRepository.GetByIdsAsync(ticketTypeIds);

            if (ticketTypes.Count != ticketTypeIds.Count)
            {
                return null;
            }

            var customer = await _customerRepository.GetByIdentityUserIdAsync(identityUserId);

            if (customer == null)
            {
                return null;
            }

            var expirationTime = dto.PaymentMethod == PaymentMethod.CashAtVenue
                                ? DateTime.UtcNow.AddHours(2)
                                : DateTime.UtcNow.AddMinutes(10);

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                EventId = dto.EventId,
                BookingTimeSlotId = dto.BookingTimeSlotId,
                CustomerId = customer.CustomerId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Pending,
                BookingCode = $"BK-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                PaymentMethod = dto.PaymentMethod,
                PaidAt = null,
                ExpiresAt = expirationTime,
            };

            foreach (var ticket in dto.Tickets)
            {
                var ticketType = ticketTypes.First(t => t.TicketTypeId == ticket.TicketTypeId);

                if (ticket.Quantity > ticketType.QuantityAvailable)
                {
                    return null;
                }

                ticketType.QuantityAvailable -= ticket.Quantity;

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
            booking.AdminFee = Math.Round(booking.TotalPrice * 0.02m, 2);

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

        public async Task<BookingRespDto?> GetByCodeAsync(string code)
        {
            var booking = await _bookingRepository.GetByCodeAsync(code);

            if (booking == null)
            {
                return null;
            }
            await ExpireIfNeededAsync(booking);

            return booking == null ? null : MapToRespDto(booking);
        }

        public async Task<bool> MarkPaidAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;
            if (booking.Status == BookingStatus.Cancelled) return false;
            if (booking.Status == BookingStatus.Expired) return false;

            booking.Status = BookingStatus.Paid;
            booking.PaidAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking);

            return true;
        }

        public async Task<bool> MarkUnpaidAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;
            if (booking.Status == BookingStatus.Cancelled) return false;
            if (booking.Status == BookingStatus.Expired) return false;

            booking.Status = BookingStatus.Pending;
            booking.PaidAt = null;
            await _bookingRepository.UpdateAsync(booking);

            return true;
        }

        public async Task<bool> CancelAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            if (booking.Status == BookingStatus.Cancelled) return true;

            foreach (var bookingTicket in booking.BookingTickets)
            {
                var ticketType = await _ticketTypeRepository.GetByIdAsync(
                    bookingTicket.TicketTypeId);

                if (ticketType != null)
                {
                    ticketType.QuantityAvailable += bookingTicket.Quantity;
                }
            }

            booking.Status = BookingStatus.Cancelled;
            await _bookingRepository.UpdateAsync(booking);

            return true;
        }

        public async Task<List<BookingRespDto>> GetMyBookingsAsync(string identityUserId)
        {
            var customer = await _customerRepository.GetByIdentityUserIdAsync(identityUserId);
            if (customer == null)
            {
                return new List<BookingRespDto>();
            }

            var bookings = await _bookingRepository.GetByCustomerIdAsync(customer.CustomerId);

            foreach (var booking in bookings)
            {
                await ExpireIfNeededAsync(booking);
            }

            return bookings.Select(MapToRespDto).ToList();
        }

        public async Task<PaymentSimulationRespDto?> SimulatePaymentAsync(Guid bookingId, bool shouldSucceed)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking == null)
            {
                return null;
            }

            if (booking.Status == BookingStatus.Pending & booking.ExpiresAt < DateTime.UtcNow)
            {
                booking.Status = BookingStatus.Expired;

                foreach (var bookingTicket in booking.BookingTickets)
                {
                    var ticketType = await _ticketTypeRepository.GetByIdAsync(
                        bookingTicket.TicketTypeId);

                    if (ticketType != null)
                    {
                        ticketType.QuantityAvailable += bookingTicket.Quantity;
                    }
                }

                await _bookingRepository.UpdateAsync(booking);

                return new PaymentSimulationRespDto
                {
                    BookingId = booking.BookingId,
                    BookingCode = booking.BookingCode,
                    Status = BookingStatus.Expired,
                    PaymentSucceeded = false,
                    Message = "Booking has expired."
                };
            }

            if (booking.PaymentMethod != PaymentMethod.Online)
            {
                return new PaymentSimulationRespDto
                {
                    BookingId = booking.BookingId,
                    BookingCode = booking.BookingCode,
                    Status = booking.Status,
                    PaymentSucceeded = false,
                    PaidAt = booking.PaidAt,
                    Message = "This booking was created for cash payment at venue and cannot be paid online."
                };
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return new PaymentSimulationRespDto
                {
                    BookingId = booking.BookingId,
                    BookingCode = booking.BookingCode,
                    Status = booking.Status,
                    PaymentSucceeded = false,
                    PaidAt = booking.PaidAt,
                    Message = "Cannot pay for a cancelled booking."
                };
            }

            if (booking.Status == BookingStatus.Paid)
            {
                return new PaymentSimulationRespDto
                {
                    BookingId = booking.BookingId,
                    BookingCode = booking.BookingCode,
                    Status = booking.Status,
                    PaymentSucceeded = true,
                    PaidAt = booking.PaidAt,
                    Message = "Booking is already paid."
                };
            }

            if (shouldSucceed)
            {
                booking.Status = BookingStatus.Paid;
                booking.PaidAt = DateTime.UtcNow;

                await _bookingRepository.UpdateAsync(booking);

                return new PaymentSimulationRespDto
                {
                    BookingId = booking.BookingId,
                    BookingCode = booking.BookingCode,
                    Status = booking.Status,
                    PaymentSucceeded = true,
                    PaidAt = booking.PaidAt,
                    Message = "Payment succeeded. Booking marked as paid."
                };
            }

            return new PaymentSimulationRespDto
            {
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode,
                Status = booking.Status,
                PaymentSucceeded = false,
                PaidAt = booking.PaidAt,
                Message = "Payment failed. Booking remains pending."
            };
        }

        private static BookingRespDto MapToRespDto(Booking booking)
        {
            return new BookingRespDto
            {
                BookingId = booking.BookingId,
                CustomerId = booking.CustomerId,
                EventId = booking.EventId,
                BookingTimeSlotId = booking.BookingTimeSlotId,
                AdminFee = (double)booking.AdminFee,
                BookingDate = booking.BookingDate,
                Status = booking.Status,
                TotalPrice = (double)booking.TotalPrice,
                Tickets = booking.BookingTickets.Select(bt => new BookingTicketRespDto
                {
                    BookingTicketId = bt.BookingTicketId,
                    TicketTypeId = bt.TicketTypeId,
                    Quantity = bt.Quantity,
                    UnitPrice = (double)bt.UnitPrice
                }).ToList(),
                BookingCode = booking.BookingCode,
                PaymentMethod = booking.PaymentMethod,
                PaidAt = booking.PaidAt
            };
        }

        private async Task ExpireIfNeededAsync(Booking booking)
        {
            if (booking.Status != BookingStatus.Pending)
            {
                return;
            }

            if (booking.ExpiresAt > DateTime.UtcNow)
            {
                return;
            }

            foreach (var bookingTicket in booking.BookingTickets)
            {
                var ticketType = await _ticketTypeRepository.GetByIdAsync(
                    bookingTicket.TicketTypeId);

                if (ticketType != null)
                {
                    ticketType.QuantityAvailable += bookingTicket.Quantity;
                }
            }

            booking.Status = BookingStatus.Expired;

            await _bookingRepository.UpdateAsync(booking);
        }

    }
}
