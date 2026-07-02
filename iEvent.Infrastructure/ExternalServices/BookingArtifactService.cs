using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace iEvent.Infrastructure.ExternalServices
{
    public class BookingArtifactService : IBookingArtifactService
    {
        public Task<string> GenerateQrCodeBase64Async(string bookingCode)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(bookingCode, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            var qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            var base64String = Convert.ToBase64String(qrCodeAsPngByteArr);

            return Task.FromResult($"data:image/png;base64,{base64String}");
        }

        public Task<byte[]> GenerateTicketPdfAsync(Booking booking)
        {
            byte[] qrCodeBytes;
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(booking.BookingCode, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                qrCodeBytes = qrCode.GetGraphic(10);
            }

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A6);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.Header().Text($"Ticket: {booking.BookingCode}")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text($"Booking Date: {booking.BookingDate:dd.MM.yyyy HH:mm}");
                        column.Item().Text($"Total payed: {booking.TotalPrice} Lei").Bold();

                        column.Item().AlignCenter().Width(100).Image(qrCodeBytes);
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return Task.FromResult(document.GeneratePdf());
        }
    }
}