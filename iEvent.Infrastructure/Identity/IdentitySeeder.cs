using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iEvent.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var roles = new[]
            {
                RoleNames.SuperAdmin,
                RoleNames.EventManager,
                RoleNames.BookingManager,
                RoleNames.Customer
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var seedSection = configuration.GetSection("SeedUsers");
            var seedUsers = new Dictionary<string, string?>
            {
                { RoleNames.SuperAdmin, seedSection["SuperAdmin:Email"] },
                { RoleNames.EventManager, seedSection["EventManager:Email"] },
                { RoleNames.BookingManager, seedSection["BookingManager:Email"] },
                { RoleNames.Customer, seedSection["Customer:Email"] }
            };

            foreach (var entry in seedUsers)
            {
                var role = entry.Key;
                var email = entry.Value;
                var password = seedSection[$"{role}:Password"];

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    continue;
                }

                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = email,
                        UserName = email
                    };

                    var createResult = await userManager.CreateAsync(user, password);
                    if (!createResult.Succeeded)
                    {
                        continue;
                    }
                }

                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
