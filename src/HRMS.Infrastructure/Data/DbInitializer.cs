using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "HR", "Manager", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create Admin user
            string adminEmail = "admin@hrms.com";
            string adminPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddToRoleAsync(adminUser, "HR"); // Also add HR role to admin
                }
            }

            // Create HR user
            string hrEmail = "hr@hrms.com";
            string hrPassword = "Hr@123";

            if (await userManager.FindByEmailAsync(hrEmail) == null)
            {
                var hrUser = new IdentityUser
                {
                    UserName = hrEmail,
                    Email = hrEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(hrUser, hrPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(hrUser, "HR");
                }
            }

            // Create Manager user
            string managerEmail = "manager@hrms.com";
            string managerPassword = "Manager@123";

            if (await userManager.FindByEmailAsync(managerEmail) == null)
            {
                var managerUser = new IdentityUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(managerUser, managerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }
        }
    }
}