using iEvent.Application.Interfaces.Repositories;
using iEvent.Domain.Entities;
using iEvent.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.AdminUsers
                .AnyAsync(a => a.IdentityUserId == identityUserId);
        }

        public async Task AddAsync(AdminUser adminUser)
        {
            _context.AdminUsers.Add(adminUser);
            await _context.SaveChangesAsync();
        }

        public async Task<AdminUser?> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.AdminUsers
                .FirstOrDefaultAsync(a => a.IdentityUserId == identityUserId);
        }
    }
}
