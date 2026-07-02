using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Venue;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;

namespace iEvent.Application.Services
{
    public class VenueImageService : IVenueImageService
    {
        private readonly ICloudinaryService _cloudinary;
        private readonly IVenueImageRepository _repo;

        public VenueImageService(
            ICloudinaryService cloudinary,
            IVenueImageRepository repo)
        {
            _cloudinary = cloudinary;
            _repo = repo;
        }

        public async Task<List<string>> UploadAsync(Guid venueId, List<FileUploadDto> files)
        {
            var urls = new List<string>();
            var images = new List<VenueImage>();

            int sort = 0;

            foreach (var file in files)
            {
                var url = await _cloudinary.UploadImageAsync(file.Content, file.FileName, "venues/gallery");

                urls.Add(url);

                images.Add(new VenueImage
                {
                    ImageId = Guid.NewGuid(),
                    VenueId = venueId,
                    Url = url,
                    SortOrder = sort++
                });
            }

            await _repo.AddRangeAsync(images);

            return urls;
        }

        public async Task<List<VenueImageRespDto>> GetByVenueIdAsync(Guid venueId)
        {
            var images = await _repo.GetByVenueIdAsync(venueId);

            return images.Select(x => new VenueImageRespDto
            {
                ImageId = x.ImageId,
                Url = x.Url,
                SortOrder = x.SortOrder
            }).ToList();
        }

        public async Task<bool> DeleteAsync(Guid imageId)
        {
            var image = await _repo.GetByIdAsync(imageId);

            if (image == null)
                return false;

            await _repo.DeleteAsync(image);
            return true;
        }
    }
}

