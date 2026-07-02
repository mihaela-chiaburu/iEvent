using iEvent.Application.DTOs.Event;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Services
{
    public interface IEventImageService
    {
        Task<List<string>> UploadAsync(Guid eventId, List<IFormFile> files);
        Task<List<EventImageRespDto>> GetByEventIdAsync(Guid eventId);
        Task<bool> DeleteAsync(Guid imageId);
    }
}
