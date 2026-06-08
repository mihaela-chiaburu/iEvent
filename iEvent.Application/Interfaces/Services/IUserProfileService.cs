using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task CreateCustomerProfileAsync( string identityUserId, string email);
        Task CreateAdminProfileAsync(string identityUserId, string email);
        Task SyncProfileAfterRoleChangeAsync( string identityUserId, string email, string role);
    }
}
