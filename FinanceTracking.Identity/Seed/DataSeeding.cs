using FinanceTracking.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace FinanceTracking.Identity.Seed
{
    public static class DataSeeding
    {
        public static void SeedMasterData(IServiceProvider provider)
        {
            IdentityDbContext context = provider.GetRequiredService<IdentityDbContext>();

            // Seed roles
            {
                var jsonData = File.ReadAllText("Seed/roles.json");
                var roles = JsonConvert.DeserializeObject<IEnumerable<IdentityRole>>(jsonData) ?? throw new Exception("Seed Role data is missing");
                IEnumerable<IdentityRole> dbRoles = context.Roles;
                IEnumerable<IdentityRole> createdRoles = roles.ExceptBy(dbRoles.Select(e => e.Name), r => r.Name);
                if (createdRoles.Any())
                {
                    context.AddRange(createdRoles);
                    context.SaveChanges();
                }
            }
        }
    }
}
