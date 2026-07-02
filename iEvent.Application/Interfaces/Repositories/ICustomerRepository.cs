using iEvent.Domain.Entities;

namespace iEvent.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid customerId);
        Task<Customer?> GetByIdentityUserIdAsync(string identityUserId);
        Task<bool> ExistsByIdentityUserIdAsync(string identityUserId);
        Task AddAsync(Customer customer);
    }
}
