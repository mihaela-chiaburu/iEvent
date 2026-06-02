using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Repositories
{
    public class TicketTypeRepository : ITicketTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TicketTypeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<TicketType>> GetAllAsync()
        {
            return _dbContext.TicketTypes.AsNoTracking().ToListAsync();
        }

        public Task<TicketType?> GetByIdAsync(Guid id)
        {
            return _dbContext.TicketTypes.FirstOrDefaultAsync(t => t.TicketTypeId == id);
        }

        public Task<List<TicketType>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return _dbContext.TicketTypes.Where(t => ids.Contains(t.TicketTypeId)).ToListAsync();
        }

        public async Task AddAsync(TicketType ticketType)
        {
            _dbContext.TicketTypes.Add(ticketType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TicketType ticketType)
        {
            _dbContext.TicketTypes.Update(ticketType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TicketType ticketType)
        {
            _dbContext.TicketTypes.Remove(ticketType);
            await _dbContext.SaveChangesAsync();
        }
    }
}
