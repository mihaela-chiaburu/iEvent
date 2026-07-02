using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iEvent.Application.DTOs.Tickets;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;

namespace iEvent.Application.Services
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly ITicketTypeRepository _ticketTypeRepository;

        public TicketTypeService(ITicketTypeRepository ticketTypeRepository)
        {
            _ticketTypeRepository = ticketTypeRepository;
        }

        public async Task<List<TicketTypeRespDto>> GetAllAsync(Guid? EventId)
        {
            var ticketTypes = await _ticketTypeRepository.GetAllAsync(EventId);
            return ticketTypes.Select(MapToRespDto).ToList();
        }

        public async Task<TicketTypeRespDto?> GetByIdAsync(Guid id)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(id);
            return ticketType == null ? null : MapToRespDto(ticketType);
        }

        public async Task<TicketTypeRespDto> CreateAsync(TicketTypeCreateDto dto)
        {
            if (dto.AvailableFrom.HasValue && dto.AvailableUntil.HasValue &&
                dto.AvailableFrom > dto.AvailableUntil)
            {
                throw new ArgumentException("AvailableFrom must be earlier than AvailableUntil.");
            }

            var ticketType = new TicketType
            {
                TicketTypeId = Guid.NewGuid(),
                EventId = dto.EventId,
                Name = dto.Name,
                Price = (decimal)dto.Price,
                QuantityAvailable = dto.QuantityAvailable,
                Icon = dto.Icon,
                AvailableFrom = dto.AvailableFrom,
                AvailableUntil = dto.AvailableUntil
            };

            await _ticketTypeRepository.AddAsync(ticketType);
            return MapToRespDto(ticketType);
        }

        public async Task<bool> UpdateAsync(Guid id, TicketTypeUpdateDto dto)
        {
            if (dto.AvailableFrom.HasValue && dto.AvailableUntil.HasValue &&
                 dto.AvailableFrom > dto.AvailableUntil)
            {
                throw new ArgumentException("AvailableFrom must be earlier than AvailableUntil.");
            }

            var ticketType = await _ticketTypeRepository.GetByIdAsync(id);
            if (ticketType == null)
            {
                return false;
            }

            ticketType.EventId = dto.EventId;
            ticketType.Name = dto.Name;
            ticketType.Price = (decimal)dto.Price;
            ticketType.QuantityAvailable = dto.QuantityAvailable;
            ticketType.Icon = dto.Icon;
            ticketType.AvailableFrom = dto.AvailableFrom;
            ticketType.AvailableUntil = dto.AvailableUntil;

            await _ticketTypeRepository.UpdateAsync(ticketType);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(id);
            if (ticketType == null)
            {
                return false;
            }

            await _ticketTypeRepository.DeleteAsync(ticketType);
            return true;
        }

        public async Task<List<string>> GetUniqueNamesAsync()
        {
            return await _ticketTypeRepository.GetUniqueNamesAsync();
        }

        private static TicketTypeRespDto MapToRespDto(TicketType ticketType)
        {
            return new TicketTypeRespDto
            {
                TicketTypeId = ticketType.TicketTypeId,
                EventId = ticketType.EventId,
                Name = ticketType.Name,
                Price = ticketType.Price,
                QuantityAvailable = ticketType.QuantityAvailable,
                Icon = ticketType.Icon,
                AvailableFrom = ticketType.AvailableFrom,
                AvailableUntil = ticketType.AvailableUntil
            };
        }
    }
}
