using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class BookingQrCodeRespDto
    {
        public string BookingCode { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
    }
}
