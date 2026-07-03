using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using iEvent.Application.DTOs.Common;
using iEvent.Application.Interfaces.Services;

namespace iEvent.Infrastructure.ExternalServices
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<CloudinaryUploadResultDto> UploadImageAsync(byte[] content, string fileName, string folder)
        {
            if (content == null || content.Length == 0)
                throw new ArgumentException("Empty file");

            await using var stream = new MemoryStream(content);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Cloudinary upload failed");

            return new CloudinaryUploadResultDto
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId
            };
        }

        public async Task DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return;

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Cloudinary delete failed");
        }
    }
}
