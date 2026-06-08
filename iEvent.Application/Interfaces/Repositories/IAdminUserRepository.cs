using iEvent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface IAdminUserRepository
    {
        Task<bool> ExistsByIdentityUserIdAsync(string identityUserId);
        Task AddAsync(AdminUser adminUser);
    }
}
