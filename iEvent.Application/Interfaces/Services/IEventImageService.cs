using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;

namespace iEvent.Application.Interfaces.Services
{
    public interface IEventImageService
    {
        Task<List<EventImageRespDto>> UploadAsync(Guid eventId, List<FileUploadDto> files);
        Task<List<EventImageRespDto>> GetByEventIdAsync(Guid eventId);
        Task<bool> DeleteAsync(Guid imageId);
        Task DeleteByEventIdAsync(Guid eventId);
        Task<EventImageRespDto> UploadBannerAsync(Guid eventId, FileUploadDto file);
    }
}
