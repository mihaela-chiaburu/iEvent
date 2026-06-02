using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEvent.Application.DTOs;

namespace iEvent.Application.Interfaces.Services
{
    public interface IEventService
    {
        Task<List<EventRespDto>> GetAllAsync(string? city);
        Task<EventRespDto?> GetByIdAsync(Guid id);
        Task<EventRespDto> CreateAsync(EventCreateDto dto);
        Task<bool> UpdateAsync(Guid id, EventUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
