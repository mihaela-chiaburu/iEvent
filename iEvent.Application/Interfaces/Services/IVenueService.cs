using iEvent.Application.DTOs;
using iEvent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Services
{
    public interface IVenueService
    {
        Task<List<VenueRespDto>> GetAllAsync();
        Task<VenueRespDto?> GetByIdAsync(Guid id);
        Task<VenueRespDto> CreateAsync(VenueCreateDto dto);
        Task<bool> UpdateAsync(Guid id, VenueUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<List<VenueRespDto>> GetPopularAsync(int take);

        Task<VenueRespDto> CreateDraftAsync();
        Task<bool> PatchAsync(Guid id, VenuePatchDto dto);
        Task<bool> PublishAsync(Guid id);
    }
}
