using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Services
{
    public interface IBookingArtifactService
    {
        Task<string> GenerateQrCodeBase64Async(string bookingCode);
        Task<byte[]> GenerateTicketPdfAsync(Booking booking);
    }
}