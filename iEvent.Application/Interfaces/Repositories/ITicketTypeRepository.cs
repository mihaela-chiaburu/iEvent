using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface ITicketTypeRepository
    {
        Task<List<TicketType>> GetAllAsync(Guid? EventId);
        Task<TicketType?> GetByIdAsync(Guid id);
        Task<List<TicketType>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task AddAsync(TicketType ticketType);
        Task UpdateAsync(TicketType ticketType);
        Task DeleteAsync(TicketType ticketType);
        Task<List<string>> GetUniqueNamesAsync();
    }
}
