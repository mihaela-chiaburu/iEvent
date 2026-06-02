using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iEvent.Application.DTOs;
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

        public async Task<List<TicketTypeRespDto>> GetAllAsync()
        {
            var ticketTypes = await _ticketTypeRepository.GetAllAsync();
            return ticketTypes.Select(MapToRespDto).ToList();
        }

        public async Task<TicketTypeRespDto?> GetByIdAsync(Guid id)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(id);
            return ticketType == null ? null : MapToRespDto(ticketType);
        }

        public async Task<TicketTypeRespDto> CreateAsync(TicketTypeCreateDto dto)
        {
            var ticketType = new TicketType
            {
                TicketTypeId = Guid.NewGuid(),
                EventId = dto.EventId,
                Name = dto.Name,
                Price = (decimal)dto.Price,
                QuantityAvailable = dto.QuantityAvailable
            };

            await _ticketTypeRepository.AddAsync(ticketType);
            return MapToRespDto(ticketType);
        }

        public async Task<bool> UpdateAsync(Guid id, TicketTypeUpdateDto dto)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(id);
            if (ticketType == null)
            {
                return false;
            }

            ticketType.EventId = dto.EventId;
            ticketType.Name = dto.Name;
            ticketType.Price = (decimal)dto.Price;
            ticketType.QuantityAvailable = dto.QuantityAvailable;

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

        private static TicketTypeRespDto MapToRespDto(TicketType ticketType)
        {
            return new TicketTypeRespDto
            {
                TicketTypeId = ticketType.TicketTypeId,
                EventId = ticketType.EventId,
                Name = ticketType.Name,
                Price = (double)ticketType.Price,
                QuantityAvailable = ticketType.QuantityAvailable
            };
        }
    }
}
