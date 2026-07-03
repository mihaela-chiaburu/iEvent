using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Venue;
using iEvent.Application.Exceptions;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;

namespace iEvent.Application.Services
{
    public class VenueImageService : IVenueImageService
    {
        private readonly ICloudinaryService _cloudinary;
        private readonly IVenueRepository _venueRepository;
        private readonly IVenueImageRepository _repo;

        public VenueImageService(
            ICloudinaryService cloudinary,
            IVenueRepository venueRepository,
            IVenueImageRepository repo)
        {
            _cloudinary = cloudinary;
            _venueRepository = venueRepository;
            _repo = repo;
        }

        public async Task<List<VenueImageRespDto>> UploadAsync(Guid venueId, List<FileUploadDto> files)
        {
            if (files == null || files.Count == 0)
                throw new ValidationException("At least one file is required.");

            var venue = await _venueRepository.GetByIdAsync(venueId);
            if (venue == null)
                throw new NotFoundException($"Venue with ID {venueId} was not found.");

            var existingImages = await _repo.GetByVenueIdAsync(venueId);
            var sort = existingImages.Count == 0 ? 0 : existingImages.Max(x => x.SortOrder) + 1;

            var images = new List<VenueImage>();
            var uploadedAssets = new List<CloudinaryUploadResultDto>();

            foreach (var file in files)
            {
                if (file == null || file.Content.Length == 0)
                    throw new ValidationException("One or more uploaded files are empty.");

                var upload = await _cloudinary.UploadImageAsync(file.Content, file.FileName, "venues/gallery");

                uploadedAssets.Add(upload);

                images.Add(new VenueImage
                {
                    ImageId = Guid.NewGuid(),
                    VenueId = venueId,
                    Url = upload.Url,
                    CloudinaryPublicId = upload.PublicId,
                    SortOrder = sort++
                });
            }

            try
            {
                await _repo.AddRangeAsync(images);
            }
            catch
            {
                foreach (var asset in uploadedAssets)
                {
                    try
                    {
                        await _cloudinary.DeleteImageAsync(asset.PublicId);
                    }
                    catch
                    {
                    }
                }

                throw;
            }

            return images.Select(x => new VenueImageRespDto
            {
                ImageId = x.ImageId,
                Url = x.Url,
                PublicId = x.CloudinaryPublicId,
                SortOrder = x.SortOrder
            }).ToList();
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

            await _cloudinary.DeleteImageAsync(image.CloudinaryPublicId);
            await _repo.DeleteAsync(image);
            return true;
        }

        public async Task DeleteByVenueIdAsync(Guid venueId)
        {
            var images = await _repo.GetByVenueIdAsync(venueId);

            foreach (var image in images)
            {
                await _cloudinary.DeleteImageAsync(image.CloudinaryPublicId);
            }

            if (images.Count > 0)
            {
                await _repo.DeleteRangeAsync(images);
            }
        }
    }
}

