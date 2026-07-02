using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;

namespace iEvent.Application.Interfaces.Services
{
    public interface IEventImageService
    {
        Task<List<string>> UploadAsync(Guid eventId, List<FileUploadDto> files);
        Task<List<EventImageRespDto>> GetByEventIdAsync(Guid eventId);
        Task<bool> DeleteAsync(Guid imageId);
    }
}
