using iEvent.Application.DTOs.Booking;
using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Payment;

namespace iEvent.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task<PagedResultDto<BookingRespDto>> GetAllAsync(BookingFilterDto filter);
        Task<BookingRespDto> GetByIdAsync(Guid id);
        Task<List<BookingRespDto>> GetMyBookingsAsync(string identityUserId);
        Task<BookingRespDto> CreateAsync(BookingCreateDto dto, string identityUserId);
        Task<BookingRespDto> CreateByManagerAsync(BookingByManagerDto dto);
        Task UpdateAsync(Guid id, BookingUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task<BookingRespDto> GetByCodeAsync(string code);
        Task UpdateTicketQuantityAsync(Guid bookingId, Guid bookingTicketId, int newQuantity);
        Task AddTicketToBookingAsync(Guid bookingId, BookingTicketAddDto dto);
        Task MarkPaidAsync(Guid id);
        Task MarkUnpaidAsync(Guid id);
        Task CancelAsync(Guid id);
        Task<PaymentSimulationRespDto> SimulatePaymentAsync(Guid bookingId, bool shouldSucceed);
        Task<BookingCollectAtVenueRespDto> CollectAtVenueAsync(Guid id, BookingCollectAtVenueDto dto, string identityUserId);
        Task<BookingQrCodeRespDto> GetQrCodeAsync(Guid id);
        Task<byte[]> GenerateTicketPdfAsync(Guid id);
    }
}
