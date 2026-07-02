using iEvent.Application.DTOs.Tickets;

namespace iEvent.Application.Interfaces.Services
{
    public interface ITicketTypeService
    {
        Task<List<TicketTypeRespDto>> GetAllAsync(Guid? EventId);
        Task<TicketTypeRespDto> GetByIdAsync(Guid id);
        Task<TicketTypeRespDto> CreateAsync(TicketTypeCreateDto dto);
        Task UpdateAsync(Guid id, TicketTypeUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task<List<string>> GetUniqueNamesAsync();
    }
}
