using iEvent.Application.DTOs.Venue;

namespace iEvent.Application.Interfaces.Services
{
    public interface IVenueService
    {
        Task<List<VenueRespDto>> GetAllAsync();
        Task<VenueRespDto> GetByIdAsync(Guid id);
        Task<VenueRespDto> CreateAsync(VenueCreateDto dto);
        Task UpdateAsync(Guid id, VenueUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task<List<VenueRespDto>> GetPopularAsync(int take);
        Task<VenueRespDto> CreateDraftAsync();
        Task PatchAsync(Guid id, VenuePatchDto dto);
        Task PublishAsync(Guid id);
    }
}
