using iEvent.Application.DTOs.Tickets;
using iEvent.Application.Exceptions;
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

        public async Task<TicketTypeRespDto> GetByIdAsync(Guid id)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(id);
            if (ticketType == null)
            {
                throw new NotFoundException($"Ticket type with ID {id} was not found.");
            }

            return MapToRespDto(ticketType);
        }

        public async Task<TicketTypeRespDto> CreateAsync(TicketTypeCreateDto dto)
        {
            if (dto.AvailableFrom.HasValue && dto.AvailableUntil.HasValue &&
                dto.AvailableFrom > dto.AvailableUntil)
            {
                throw new ValidationException("AvailableFrom must be earlier than AvailableUntil.");
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

        public async Task UpdateAsync(Guid id, TicketTypeUpdateDto dto)
        {
            if (dto.AvailableFrom.HasValue && dto.AvailableUntil.HasValue &&
                 dto.AvailableFrom > dto.AvailableUntil)
            {
                throw new ValidationException("AvailableFrom must be earlier than AvailableUntil.");
            }

            var ticketType = await _ticketTypeRepository.GetByIdAsync(id);
            if (ticketType == null)
            {
                throw new NotFoundException($"Ticket type with ID {id} was not found.");
            }

            ticketType.EventId = dto.EventId;
            ticketType.Name = dto.Name;
            ticketType.Price = (decimal)dto.Price;
            ticketType.QuantityAvailable = dto.QuantityAvailable;
            ticketType.Icon = dto.Icon;
            ticketType.AvailableFrom = dto.AvailableFrom;
            ticketType.AvailableUntil = dto.AvailableUntil;

            await _ticketTypeRepository.UpdateAsync(ticketType);
        }

        public async Task DeleteAsync(Guid id)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(id);
            if (ticketType == null)
            {
                throw new NotFoundException($"Ticket type with ID {id} was not found.");
            }

            await _ticketTypeRepository.DeleteAsync(ticketType);
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
