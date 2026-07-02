using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;
using iEvent.Domain.Enums;

namespace iEvent.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAdminUserRepository _adminUserRepository;

        public UserProfileService(ICustomerRepository customerRepository,
            IAdminUserRepository adminUserRepository)
        {
            _customerRepository = customerRepository;
            _adminUserRepository = adminUserRepository;
        }

        public async Task CreateCustomerProfileAsync(string identityUserId, string email, string name, string? phoneNumber)
        {
            var exists = await _customerRepository
                .ExistsByIdentityUserIdAsync(identityUserId);

            if (exists)
            {
                return;
            }

            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                IdentityUserId = identityUserId,
                Email = email,
                Name = name,
                PhoneNumber = phoneNumber
            };

            await _customerRepository.AddAsync(customer);
        }

        public async Task CreateAdminProfileAsync(string identityUserId, string email, string name, string? phoneNumber)
        {
            var exists = await _adminUserRepository
                .ExistsByIdentityUserIdAsync(identityUserId);

            if (exists)
            {
                return;
            }

            var admin = new AdminUser
            {
                AdminId = Guid.NewGuid(),
                IdentityUserId = identityUserId,
                Email = email,
                Name = name,
                PhoneNumber = phoneNumber
            };

            await _adminUserRepository.AddAsync(admin);
        }

        public async Task SyncProfileAfterRoleChangeAsync(string identityUserId, string email, string name, string? phoneNumber, string role)
        {
            if (role == RoleNames.Customer)
            {
                await CreateCustomerProfileAsync(identityUserId, email, name, phoneNumber);
            }
            else
            {
                await CreateAdminProfileAsync(identityUserId, email, name, phoneNumber);
            }
        }
    }
}
