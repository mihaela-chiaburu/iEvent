using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(Guid customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public async Task<bool> ExistsByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Customers
                .AnyAsync(c => c.IdentityUserId == identityUserId);
        }

        public async Task AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId);
        }
    }
}
