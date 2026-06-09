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
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<EventRespDto> CreateAsync(EventCreateDto dto)
        {
            var ievent = new Event
            {
                EventId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                VenueId = dto.VenueId,
                ImageUrl = dto.ImageUrl,
                Category = dto.Category
            };

            await _eventRepository.AddAsync(ievent);

            return MapToRespDto(ievent);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var ievent = await _eventRepository.GetByIdAsync(id);
            if (ievent == null)
            {
                return false;
            }

            await _eventRepository.DeleteAsync(ievent);
            return true;
        }

        public async Task<List<EventRespDto>> GetAllAsync(string? city)
        {
            var ievent = await _eventRepository.GetAllAsync(city);
            return ievent.Select(MapToRespDto).ToList();
        }

        public async Task<EventRespDto?> GetByIdAsync(Guid id)
        {
            var ievent = await _eventRepository.GetByIdAsync(id);

            return ievent == null ? null : MapToRespDto(ievent);
        }

        public async Task<bool> UpdateAsync(Guid id, EventUpdateDto dto)
        {
            var ievent = await _eventRepository.GetByIdAsync(id);
            if(ievent == null)
            {
                return false;
            }

            ievent.Name = dto.Name;
            ievent.Description = dto.Description;
            ievent.StartDate = dto.StartDate;
            ievent.EndDate = dto.EndDate;
            ievent.VenueId = dto.VenueId;
            ievent.ImageUrl = dto.ImageUrl;
            ievent.Category = dto.Category;

            await _eventRepository.UpdateAsync(ievent);
            return true;
        }

        private static EventRespDto MapToRespDto(Event ievent)
        {
            return new EventRespDto
            {
                EventId = ievent.EventId,
                Name = ievent.Name,
                Description = ievent.Description,
                StartDate = ievent.StartDate,
                EndDate = ievent.EndDate,
                VenueId = ievent.VenueId,
                ImageUrl = ievent.ImageUrl,
                Category = ievent.Category,
            };
        }
    }
}
