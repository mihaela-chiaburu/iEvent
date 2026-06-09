using iEvent.Application.DTOs;
using iEvent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Services
{
    public interface IEventService
    {
        Task<List<EventRespDto>> GetAllAsync(string? city, Guid? venueId, EventCategory? category, DateTime? fromDate, DateTime? toDate);
        Task<EventRespDto?> GetByIdAsync(Guid id);
        Task<EventRespDto> CreateAsync(EventCreateDto dto);
        Task<bool> UpdateAsync(Guid id, EventUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
