using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;
using iEvent.Domain.Enums;
using QRCoder;
using System.Drawing;

namespace iEvent.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IAdminUserRepository _adminUserRepository;

        public BookingService(IBookingRepository bookingRepository, ITicketTypeRepository ticketTypeRepository,
            ICustomerRepository customerRepository, IEventRepository eventRepository, IAdminUserRepository adminUserRepository)
        {
            _bookingRepository = bookingRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _customerRepository = customerRepository;
            _eventRepository = eventRepository;
            _adminUserRepository = adminUserRepository;
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
                throw new ArgumentException("No tickets provided.");

            if (dto.Tickets.Any(t => t.Quantity <= 0))
                throw new ArgumentException("Ticket quantity must be greater than 0.");

            var ievent = await _eventRepository.GetByIdAsync(dto.EventId);
            if (ievent == null)
                throw new KeyNotFoundException($"Event with ID {dto.EventId} was not found (it might be a draft or deleted).");

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
                throw new KeyNotFoundException("One or more ticket types were not found in the database.");
            }

            var customer = await _customerRepository.GetByIdentityUserIdAsync(identityUserId);

            if (customer == null)
            {
                throw new KeyNotFoundException($"No Customer profile found associated with the logged in user: (IdentityUserId: {identityUserId}).");
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
                    throw new ArgumentException($"Insufficient stock for the ticket '{ticketType.Name}'. Available: {ticketType.QuantityAvailable}, Requested: {ticket.Quantity}");
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

        public async Task<BookingRespDto?> CreateByManagerAsync(BookingByManagerDto dto)
        {
            if (dto.Tickets == null || !dto.Tickets.Any())
                throw new ArgumentException("No tickets provided.");

            if (dto.Tickets.Any(t => t.Quantity <= 0))
                throw new ArgumentException("Ticket quantity must be greater than 0.");

            var ievent = await _eventRepository.GetByIdAsync(dto.EventId);
            if (ievent == null)
                throw new KeyNotFoundException($"Event with ID {dto.EventId} was not found.");

            var timeSlotExists = ievent.EventDates
                .SelectMany(ed => ed.TimeSlots)
                .Any(ts => ts.TimeSlotId == dto.BookingTimeSlotId);

            if (!timeSlotExists) return null;

            Guid finalCustomerId;

            if (dto.CustomerId.HasValue)
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(dto.CustomerId.Value);
                if (existingCustomer == null)
                    throw new KeyNotFoundException($"Customer with ID {dto.CustomerId.Value} not found.");

                finalCustomerId = existingCustomer.CustomerId;
            }
            else if (dto.NewCustomer != null)
            {
                if (string.IsNullOrWhiteSpace(dto.NewCustomer.Name) || string.IsNullOrWhiteSpace(dto.NewCustomer.Email))
                    throw new ArgumentException("For a new customer, Name and Email are required.");

                var newCustomer = new Customer
                {
                    CustomerId = Guid.NewGuid(),
                    Name = dto.NewCustomer.Name,
                    Email = dto.NewCustomer.Email,
                    PhoneNumber = dto.NewCustomer.Phone,
                    IdentityUserId = string.Empty
                };

                await _customerRepository.AddAsync(newCustomer);
                finalCustomerId = newCustomer.CustomerId;
            }
            else
            {
                throw new ArgumentException("You must either provide an existing CustomerId or complete NewCustomer details.");
            }

            var ticketTypeIds = dto.Tickets.Select(t => t.TicketTypeId).Distinct().ToList();
            var ticketTypes = await _ticketTypeRepository.GetByIdsAsync(ticketTypeIds);

            if (ticketTypes.Count != ticketTypeIds.Count)
                throw new KeyNotFoundException("One or more ticket types were not found.");

            var expirationTime = dto.PaymentMethod == PaymentMethod.CashAtVenue
                ? DateTime.UtcNow.AddHours(2)
                : DateTime.UtcNow.AddMinutes(10);

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                EventId = dto.EventId,
                BookingTimeSlotId = dto.BookingTimeSlotId,
                CustomerId = finalCustomerId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Pending,
                BookingCode = $"MGR-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                PaymentMethod = dto.PaymentMethod,
                PaidAt = null,
                ExpiresAt = expirationTime,
            };

            foreach (var ticket in dto.Tickets)
            {
                var ticketType = ticketTypes.First(t => t.TicketTypeId == ticket.TicketTypeId);

                if (ticket.Quantity > ticketType.QuantityAvailable)
                    throw new ArgumentException($"Insufficient stock for '{ticketType.Name}'. Available: {ticketType.QuantityAvailable}");

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

        public async Task<PagedResultDto<BookingRespDto>> GetAllAsync(BookingFilterDto filter)
        {
            var (bookings, totalCount) = await _bookingRepository.GetPagedAsync(filter);

            foreach (var booking in bookings)
            {
                await ExpireIfNeededAsync(booking);
            }

            return new PagedResultDto<BookingRespDto>
            {
                Items = bookings.Select(MapToRespDto).ToList(),
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
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

        public async Task<bool> UpdateTicketQuantityAsync(Guid bookingId, Guid bookingTicketId, int newQuantity)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return false;
            }

            var ticket = booking.BookingTickets.FirstOrDefault(bt => bt.BookingTicketId == bookingTicketId);
            if (ticket == null)
            {
                return false;
            }

            ticket.Quantity = newQuantity;

            booking.TotalPrice = booking.BookingTickets.Sum(bt => bt.UnitPrice * bt.Quantity);

            booking.AdminFee = Math.Round(booking.TotalPrice * 0.02m, 2);

            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> AddTicketToBookingAsync(Guid bookingId, BookingTicketAddDto dto)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with id {bookingId} not found.");
            }

            var ticketType = await _ticketTypeRepository.GetByIdAsync(dto.TicketTypeId);
            if (ticketType == null)
            {
                throw new KeyNotFoundException($"Ticket type with ID {dto.TicketTypeId} doesn't exist.");
            }

            if (dto.Quantity > ticketType.QuantityAvailable)
            {
                throw new ArgumentException($"Insufficient stock for the ticket '{ticketType.Name}'. Available: {ticketType.QuantityAvailable}, Requested: {dto.Quantity}");
            }

            ticketType.QuantityAvailable -= dto.Quantity;

            var existingTicket = booking.BookingTickets.FirstOrDefault(bt => bt.TicketTypeId == dto.TicketTypeId);

            if (existingTicket != null)
            {
                existingTicket.Quantity += dto.Quantity;
            }
            else
            {
                var newBookingTicket = new BookingTicket
                {
                    BookingTicketId = Guid.NewGuid(),
                    BookingId = booking.BookingId,
                    TicketTypeId = dto.TicketTypeId,
                    Quantity = dto.Quantity,
                    UnitPrice = ticketType.Price
                };

                booking.BookingTickets.Add(newBookingTicket);
            }

            booking.TotalPrice = booking.BookingTickets.Sum(bt => bt.UnitPrice * bt.Quantity);
            booking.AdminFee = Math.Round(booking.TotalPrice * 0.02m, 2);

            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<BookingCollectAtVenueRespDto?> CollectAtVenueAsync(Guid id, BookingCollectAtVenueDto dto, string identityUserId)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return null;

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Expired)
                throw new InvalidOperationException($"Cannot collect money for a booking with status {booking.Status}.");

            if (booking.Status == BookingStatus.Paid)
                throw new InvalidOperationException("This booking is already paid.");

            var expectedTotal = booking.TotalPrice + booking.AdminFee;
            if (dto.Amount < expectedTotal)
                throw new ArgumentException($"The collected amount ({dto.Amount}) is less than the required total ({expectedTotal}).");

            var admin = await _adminUserRepository.GetByIdentityUserIdAsync(identityUserId);
            if (admin == null)
                throw new KeyNotFoundException($"No Admin profile found associated with IdentityUserId: {identityUserId}.");

            booking.Status = BookingStatus.Paid;
            booking.PaidAt = dto.CollectedAt;
            booking.CollectedById = admin.AdminId;
            booking.CollectedAmount = dto.Amount;

            await _bookingRepository.UpdateAsync(booking);

            var baseDto = MapToRespDto(booking);

            return new BookingCollectAtVenueRespDto
            {
                BookingId = baseDto.BookingId,
                CustomerId = baseDto.CustomerId,
                EventId = baseDto.EventId,
                BookingTimeSlotId = baseDto.BookingTimeSlotId,
                AdminFee = baseDto.AdminFee,
                BookingDate = baseDto.BookingDate,
                Status = baseDto.Status,
                TotalPrice = baseDto.TotalPrice,
                Tickets = baseDto.Tickets,
                BookingCode = baseDto.BookingCode,
                PaymentMethod = baseDto.PaymentMethod,
                PaidAt = baseDto.PaidAt,
                CollectedById = admin.AdminId,
                CollectedAmount = (double)dto.Amount
            };
        }

        public async Task<BookingQrCodeRespDto?> GetQrCodeAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return null;

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(booking.BookingCode, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);

                string base64String = Convert.ToBase64String(qrCodeAsPngByteArr);

                return new BookingQrCodeRespDto
                {
                    BookingCode = booking.BookingCode,
                    QrCodeBase64 = $"data:image/png;base64,{base64String}"
                };
            }
        }
    }
}
