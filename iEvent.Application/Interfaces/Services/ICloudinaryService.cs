namespace iEvent.Application.Interfaces.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(byte[] content, string fileName, string folder);
    }
}
