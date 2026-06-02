using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Application.DTOs;

namespace iEvent.Application.Interfaces.Services
{
    public interface IVenueService
    {
        Task<List<VenueRespDto>> GetAllAsync();
        Task<VenueRespDto?> GetByIdAsync(Guid id);
        Task<VenueRespDto> CreateAsync(VenueCreateDto dto);
        Task<bool> UpdateAsync(Guid id, VenueUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
