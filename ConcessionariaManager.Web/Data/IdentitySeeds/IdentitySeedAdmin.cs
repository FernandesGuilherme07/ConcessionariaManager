
using Microsoft.AspNetCore.Identity;

namespace ConcessionariaManager.Web.Data.IdentitySeeds
{
    public static class IdentitySeed
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await AddRoles(serviceProvider);

            var adminEmail = "admin@email.com";
            var adminPassword = "Admin@123";
            var adminRole = "Administrador";

            // Verifica se o usuário já existe
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
                else
                {
                    throw new Exception("Erro ao criar usuário admin: " + string.Join(", ", result.Errors));
                }
            }
        }

        private static async Task AddRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new List<string>
            {
                "Administrador",
                "Gerente",
                "Vendedor"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
