using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;

namespace iEvent.Application.Services
{
    public class EventImageService : IEventImageService
    {
        private readonly ICloudinaryService _cloudinary;
        private readonly IEventImageRepository _repo;

        public EventImageService(
            ICloudinaryService cloudinary,
            IEventImageRepository repo)
        {
            _cloudinary = cloudinary;
            _repo = repo;
        }

        public async Task<List<string>> UploadAsync(Guid eventId, List<FileUploadDto> files)
        {
            var urls = new List<string>();
            var images = new List<EventImage>();

            int sort = 0;

            foreach (var file in files)
            {
                var url = await _cloudinary.UploadImageAsync(file.Content, file.FileName, "events/gallery");

                urls.Add(url);

                images.Add(new EventImage
                {
                    ImageId = Guid.NewGuid(),
                    EventId = eventId,
                    Url = url,
                    SortOrder = sort++
                });
            }

            await _repo.AddRangeAsync(images);

            return urls;
        }

        public async Task<List<EventImageRespDto>> GetByEventIdAsync(Guid eventId)
        {
            var images = await _repo.GetByEventIdAsync(eventId);

            return images.Select(x => new EventImageRespDto
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
