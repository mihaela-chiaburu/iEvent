using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task CreateCustomerProfileAsync(string identityUserId, string email, string Name, string? phoneNumber);
        Task CreateAdminProfileAsync(string identityUserId, string email, string Name, string? phoneNumber);
        Task SyncProfileAfterRoleChangeAsync(string identityUserId, string email, string name, string? phoneNumber, string role);
    }
}
