namespace iEvent.Application.DTOs.Common
{
    public class FileUploadDto
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
    }
}