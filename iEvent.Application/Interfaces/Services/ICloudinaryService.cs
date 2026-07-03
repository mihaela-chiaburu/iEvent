using iEvent.Application.DTOs.Common;

namespace iEvent.Application.Interfaces.Services
{
    public interface ICloudinaryService
    {
        Task<CloudinaryUploadResultDto> UploadImageAsync(byte[] content, string fileName, string folder);
        Task DeleteImageAsync(string publicId);
    }
}
