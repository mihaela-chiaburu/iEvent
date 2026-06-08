using iEvent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdentityUserIdAsync(string identityUserId);
        Task<bool> ExistsByIdentityUserIdAsync(string identityUserId);
        Task AddAsync(Customer customer);
    }
}
