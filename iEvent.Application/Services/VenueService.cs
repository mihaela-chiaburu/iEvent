using iEvent.Application.DTOs.Venue;
using iEvent.Application.Exceptions;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;
using iEvent.Domain.ValueObjects;

namespace iEvent.Application.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IVenueImageService _venueImageService;
        private readonly ICloudinaryService _cloudinary;

        public VenueService(
            IVenueRepository venueRepository,
            IVenueImageService venueImageService,
            ICloudinaryService cloudinary)
        {
            _venueRepository = venueRepository;
            _venueImageService = venueImageService;
            _cloudinary = cloudinary;
        }

        public async Task<List<VenueRespDto>> GetAllAsync()
        {
            var venues = await _venueRepository.GetAllAsync();

            return venues.Select(MapToRespDto).ToList();
        }

        public async Task<VenueRespDto> GetByIdAsync(Guid id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                throw new NotFoundException($"Venue with ID {id} was not found.");
            }

            return MapToRespDto(venue);
        }

        public async Task<VenueRespDto> CreateAsync(VenueCreateDto dto)
        {
            var venue = new Venue
            {
                VenueId = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                Capacity = dto.Capacity,
                Description = dto.Description,
                Phone = dto.Phone,
                Email = dto.Email,
                MapLocation = new MapLocation(dto.Latitude, dto.Longitude),
                Facilities = new List<VenueFacility>(),
                Images = new List<VenueImage>()
            };

            var venueId = venue.VenueId;

            venue.Facilities = dto.Facilities?
                .Select(f => new VenueFacility
                {
                    FacilityId = Guid.NewGuid(),
                    VenueId = venueId,
                    Name = f.Name
                })
                .ToList() ?? new List<VenueFacility>();

            venue.Images = dto.Images?
                .Select(i => new VenueImage
                {
                    ImageId = Guid.NewGuid(),
                    VenueId = venueId,
                    Url = i.Url,
                    CloudinaryPublicId = i.PublicId,
                    SortOrder = i.SortOrder
                })
                .ToList() ?? new List<VenueImage>();

            await _venueRepository.AddAsync(venue);

            return MapToRespDto(venue);
        }

        public async Task UpdateAsync(Guid id, VenueUpdateDto dto)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                throw new NotFoundException($"Venue with ID {id} was not found.");
            }

            venue.Name = dto.Name;
            venue.Address = dto.Address;
            venue.City = dto.City;
            venue.Capacity = dto.Capacity;
            venue.Description = dto.Description;
            venue.Phone = dto.Phone;
            venue.Email = dto.Email;
            venue.MapLocation = new MapLocation(dto.Latitude, dto.Longitude);

            venue.Facilities = dto.Facilities.Select(f => new VenueFacility
            {
                FacilityId = Guid.NewGuid(),
                VenueId = venue.VenueId,
                Name = f.Name
            }).ToList();

            venue.Images = dto.Images.Select(i => new VenueImage
            {
                ImageId = Guid.NewGuid(),
                VenueId = venue.VenueId,
                Url = i.Url,
                CloudinaryPublicId = i.PublicId,
                SortOrder = i.SortOrder
            }).ToList();

            await _venueRepository.UpdateAsync(venue);
        }

        public async Task DeleteAsync(Guid id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                throw new NotFoundException($"Venue with ID {id} was not found.");
            }

            foreach (var image in venue.Images)
            {
                if (!string.IsNullOrWhiteSpace(image.CloudinaryPublicId))
                {
                    await _cloudinary.DeleteImageAsync(image.CloudinaryPublicId);
                }
            }

            await _venueRepository.DeleteAsync(venue);
        }

        public async Task<List<VenueRespDto>> GetPopularAsync(int take = 10)
        {
            var venues = await _venueRepository.GetPopularAsync(take);
            return venues.Select(MapToRespDto).ToList();
        }

        private static VenueRespDto MapToRespDto(Venue venue)
        {
            return new VenueRespDto
            {
                VenueId = venue.VenueId,
                Name = venue.Name,
                Address = venue.Address,
                City = venue.City,
                Capacity = venue.Capacity,
                Description = venue.Description,
                Phone = venue.Phone,
                Email = venue.Email,
                Latitude = venue.MapLocation.Latitude,
                Longitude = venue.MapLocation.Longitude,
                EventCount = venue.Events.Count(e => !e.IsDraft),
                Facilities = venue.Facilities.Select(f => new VenueFacilityRespDto
                {
                    FacilityId = f.FacilityId,
                    Name = f.Name
                }).ToList(),

                Images = venue.Images
                    .OrderBy(i => i.SortOrder) 
                    .Select(i => new VenueImageRespDto
                    {
                        ImageId = i.ImageId,
                        Url = i.Url,
                        PublicId = i.CloudinaryPublicId,
                        SortOrder = i.SortOrder
                    }).ToList()
            };
        }

        public async Task<VenueRespDto> CreateDraftAsync()
        {
            var venue = new Venue
            {
                VenueId = Guid.NewGuid(),
                Name = "",
                City = "",
                Address = "",
                Capacity = 0,
                MapLocation = new MapLocation(0, 0),
                Description = "",
                Phone = "",
                Email = "",
                Events = new List<Event>(),
                Facilities = new List<VenueFacility>(),
                Images = new List<VenueImage>(),
                IsDraft = true,
            };

            await _venueRepository.AddAsync(venue);

            return MapToRespDto(venue);
        }

        public async Task PatchAsync(Guid id, VenuePatchDto dto)
        {
            var ven = await _venueRepository.GetByIdAsync(id);
            if (ven == null)
            {
                throw new NotFoundException($"Venue with ID {id} was not found.");
            }

            ven.Name = dto.Name;
            ven.Address = dto.Address;
            ven.City = dto.City;
            ven.Capacity = dto.Capacity;
            ven.MapLocation = new MapLocation(dto.Latitude, dto.Longitude);
            ven.Description = dto.Description;
            ven.Phone = dto.Phone;
            ven.Email = dto.Email;

            var facilities = new List<VenueFacility>();
            var newImages = new List<VenueImage>();

            if (dto.Facilities != null)
            {
                facilities = dto.Facilities.Select(f => new VenueFacility
                {
                    FacilityId = Guid.NewGuid(),
                    VenueId = ven.VenueId,
                    Name = f.Name
                }).ToList();
            }

            if (dto.Images != null)
            {
                newImages = dto.Images.Select(i => new VenueImage
                {
                    ImageId = Guid.NewGuid(),
                    VenueId = ven.VenueId,
                    Url = i.Url,
                    CloudinaryPublicId = i.PublicId, 
                    SortOrder = i.SortOrder
                }).ToList();
            }

            await _venueRepository.ReplaceVenueChildrenAsync(ven.VenueId, facilities, newImages);

            ven.IsDraft = false;
            await _venueRepository.UpdateAsync(ven);
        }

        public async Task PublishAsync(Guid id)
        {
            var ev = await _venueRepository.GetByIdAsync(id);
            if (ev == null)
            {
                throw new NotFoundException($"Venue with ID {id} was not found.");
            }

            if (string.IsNullOrWhiteSpace(ev.Name))
                throw new InvalidOperationException("Venue name is required before publishing.");

            if (string.IsNullOrWhiteSpace(ev.Address))
                throw new InvalidOperationException("Venue address is required before publishing.");

            if (string.IsNullOrWhiteSpace(ev.City))
                throw new InvalidOperationException("Venue city is required before publishing.");

            if (ev.Capacity <= 0)
                throw new InvalidOperationException("Venue capacity must be greater than zero before publishing.");

            ev.IsDraft = false;

            await _venueRepository.UpdateAsync(ev);
        }
    }
}
