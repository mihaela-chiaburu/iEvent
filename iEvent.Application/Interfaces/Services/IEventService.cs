using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;

namespace iEvent.Application.Interfaces.Services
{
    public interface IEventService
    {
        Task<PagedResultDto<EventRespDto>> GetAllAsync(EventQueryDto query);
        Task<EventRespDto> GetByIdAsync(Guid id);
        Task<EventRespDto> CreateAsync(EventCreateDto dto);
        Task DeleteAsync(Guid id);
        Task<List<EventRespDto>> GetEventsByVenueIdAsync(Guid venueId);
        Task<List<EventRespDto>> GetPopularEventsAsync(int count);
        Task<List<EventDateRespDto>> GetEventDatesAsync(Guid id);
        Task AddEventDatesAsync(Guid id, List<EventDateCreateDto> dto);
        Task<List<EventRespDto>> GetSimilarEventsAsync(Guid id, int count = 4);
        Task<List<EventRespDto>> GetPreviewByCategoryAsync(string category, int count = 4);
        Task<List<EventRespDto>> GetPreviewByCityAsync(string city, int count = 4);
        Task<EventRespDto> CreateDraftAsync();
        Task PatchAsync(Guid id, EventPatchDto dto);
        Task PublishAsync(Guid id);
    }
}
