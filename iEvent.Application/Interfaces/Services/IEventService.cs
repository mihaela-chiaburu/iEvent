using iEvent.Application.DTOs;
using iEvent.Application.DTOs.Event;
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
        Task<PagedResult<EventRespDto>> GetAllAsync(EventQueryDto query);
        Task<EventRespDto?> GetByIdAsync(Guid id);
        Task<EventRespDto> CreateAsync(EventCreateDto dto);
        Task<bool> UpdateAsync(Guid id, EventUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<List<EventRespDto>> GetEventsByVenueIdAsync(Guid venueId);
        Task<List<EventRespDto>> GetPopularEventsAsync(int count);
        Task<List<EventDateRespDto>?> GetEventDatesAsync(Guid id);
        Task<bool> AddEventDatesAsync(Guid id, List<EventDateCreateDto> dto);
        Task<List<EventRespDto>?> GetSimilarEventsAsync(Guid id, int count = 4);
        Task<List<EventRespDto>> GetPreviewByCategoryAsync(string category, int count = 4);
        Task<List<EventRespDto>> GetPreviewByCityAsync(string city, int count = 4);

        Task<EventRespDto> CreateDraftAsync();
        Task<bool> PatchAsync(Guid id, EventPatchDto dto);
        Task<bool> PublishAsync(Guid id);
    }
}
