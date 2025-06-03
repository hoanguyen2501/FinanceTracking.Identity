using FinanceTracking.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracking.Identity.Data
{
    public sealed class IdentityDbContext : IdentityDbContext<AppUser>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new()
            {
                new()
                {
                    Name = "Admid",
                    NormalizedName = "ADMIN"
                },
                new()
                {
                    Name = "User",
                    NormalizedName = "User"
                },
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}