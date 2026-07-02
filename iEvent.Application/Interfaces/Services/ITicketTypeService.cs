using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Application.DTOs.Tickets;

namespace iEvent.Application.Interfaces.Services
{
    public interface ITicketTypeService
    {
        Task<List<TicketTypeRespDto>> GetAllAsync(Guid? EventId);
        Task<TicketTypeRespDto?> GetByIdAsync(Guid id);
        Task<TicketTypeRespDto> CreateAsync(TicketTypeCreateDto dto);
        Task<bool> UpdateAsync(Guid id, TicketTypeUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<List<string>> GetUniqueNamesAsync();
    }
}
