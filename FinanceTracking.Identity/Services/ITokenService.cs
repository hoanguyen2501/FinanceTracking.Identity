using FinanceTracking.Identity.Entities;

namespace FinanceTracking.Identity.Services
{
    public interface ITokenService
    {
        string GenerateToken(AppUser appUser);
    }
}