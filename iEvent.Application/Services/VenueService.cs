using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;
using iEvent.Domain.ValueObjects;

namespace iEvent.Application.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;

        public VenueService(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task<List<VenueRespDto>> GetAllAsync()
        {
            var venues = await _venueRepository.GetAllAsync();

            return venues.Select(MapToRespDto).ToList();
        }

        public async Task<VenueRespDto?> GetByIdAsync(Guid id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            return venue == null ? null : MapToRespDto(venue);
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
                    SortOrder = i.SortOrder
                })
                .ToList() ?? new List<VenueImage>();

            await _venueRepository.AddAsync(venue);

            return MapToRespDto(venue);
        }

        public async Task<bool> UpdateAsync(Guid id, VenueUpdateDto dto)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                return false;
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
                SortOrder = i.SortOrder
            }).ToList();

            await _venueRepository.UpdateAsync(venue);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                return false;
            }

            await _venueRepository.DeleteAsync(venue);
            return true;
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
                        SortOrder = i.SortOrder
                    }).ToList()
            };
        }
    }
}
