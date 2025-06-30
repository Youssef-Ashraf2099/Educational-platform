using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace YourNamespace.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Learner", "Instructor" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create a default user and assign roles
            var user = new IdentityUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com"
            };

            string userPassword = "Password123!";
            var userExists = await userManager.FindByEmailAsync(user.Email);

            if (userExists == null)
            {
                var createUser = await userManager.CreateAsync(user, userPassword);
                if (createUser.Succeeded)
                {
                    // Assign the user to the "Admin" role
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
