using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IAdminUserRepository
    {
        Task<bool> ExistsByIdentityUserIdAsync(string identityUserId);
        Task<AdminUser?> GetByIdentityUserIdAsync(string identityUserId);
        Task AddAsync(AdminUser adminUser);
    }
}
