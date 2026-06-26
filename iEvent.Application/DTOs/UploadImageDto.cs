using Microsoft.AspNetCore.Http;

namespace iEvent.Application.DTOs
{
    public class UploadImageDto
    {
        public IFormFile File { get; set; } = default!;
        public string Folder { get; set; } = string.Empty;
    }
}
