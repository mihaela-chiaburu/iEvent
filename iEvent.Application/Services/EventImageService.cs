using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;
using iEvent.Application.Exceptions;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;

namespace iEvent.Application.Services
{
    public class EventImageService : IEventImageService
    {
        private readonly ICloudinaryService _cloudinary;
        private readonly IEventRepository _eventRepository;
        private readonly IEventImageRepository _repo;

        public EventImageService(
            ICloudinaryService cloudinary,
            IEventRepository eventRepository,
            IEventImageRepository repo)
        {
            _cloudinary = cloudinary;
            _eventRepository = eventRepository;
            _repo = repo;
        }

        public async Task<List<EventImageRespDto>> UploadAsync(Guid eventId, List<FileUploadDto> files)
        {
            if (files == null || files.Count == 0)
                throw new ValidationException("At least one file is required.");

            var ievent = await _eventRepository.GetByIdAsync(eventId);
            if (ievent == null)
                throw new NotFoundException($"Event with ID {eventId} was not found.");

            var existingImages = await _repo.GetByEventIdAsync(eventId);
            var sort = existingImages.Count == 0 ? 0 : existingImages.Max(x => x.SortOrder) + 1;

            var images = new List<EventImage>();
            var uploadedAssets = new List<CloudinaryUploadResultDto>();

            foreach (var file in files)
            {
                if (file == null || file.Content.Length == 0)
                    throw new ValidationException("One or more uploaded files are empty.");

                var upload = await _cloudinary.UploadImageAsync(file.Content, file.FileName, "events/gallery");

                uploadedAssets.Add(upload);

                images.Add(new EventImage
                {
                    ImageId = Guid.NewGuid(),
                    EventId = eventId,
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

            return images.Select(x => new EventImageRespDto
            {
                ImageId = x.ImageId,
                Url = x.Url,
                PublicId = x.CloudinaryPublicId,
                SortOrder = x.SortOrder
            }).ToList();
        }

        public async Task<List<EventImageRespDto>> GetByEventIdAsync(Guid eventId)
        {
            var images = await _repo.GetByEventIdAsync(eventId);

            return images.Select(x => new EventImageRespDto
            {
                ImageId = x.ImageId,
                Url = x.Url,
                PublicId = x.CloudinaryPublicId,
                SortOrder = x.SortOrder,
                IsBanner = x.IsBanner
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

        public async Task DeleteByEventIdAsync(Guid eventId)
        {
            var images = await _repo.GetByEventIdAsync(eventId);

            foreach (var image in images)
            {
                await _cloudinary.DeleteImageAsync(image.CloudinaryPublicId);
            }

            if (images.Count > 0)
            {
                await _repo.DeleteRangeAsync(images);
            }
        }

        public async Task<EventImageRespDto> UploadBannerAsync(Guid eventId, FileUploadDto file)
        {
            if (file == null || file.Content.Length == 0)
                throw new ValidationException("Banner file is required.");

            var ievent = await _eventRepository.GetByIdAsync(eventId);
            if (ievent == null)
                throw new NotFoundException($"Event with ID {eventId} was not found.");

            var currentBanner = await _repo.GetBannerByEventIdAsync(eventId);

            var upload = await _cloudinary.UploadImageAsync(file.Content, file.FileName, "events/banner");

            var newBanner = new EventImage
            {
                ImageId = Guid.NewGuid(),
                EventId = eventId,
                Url = upload.Url,
                CloudinaryPublicId = upload.PublicId,
                SortOrder = 0,
                IsBanner = true
            };

            try
            {
                if (currentBanner != null)
                {
                    await _cloudinary.DeleteImageAsync(currentBanner.CloudinaryPublicId);
                    await _repo.DeleteAsync(currentBanner);
                }

                await _repo.AddRangeAsync(new List<EventImage> { newBanner });
            }
            catch
            {
                await _cloudinary.DeleteImageAsync(upload.PublicId);
                throw;
            }

            return new EventImageRespDto
            {
                ImageId = newBanner.ImageId,
                Url = newBanner.Url,
                PublicId = newBanner.CloudinaryPublicId,
                SortOrder = newBanner.SortOrder,
                IsBanner = true
            };
        }
    }
}
