using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinanceTracking.Identity.Entities;
using FinanceTracking.Identity.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracking.Identity.Services
{
    public sealed class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IServiceProvider provider)
        {
            _configuration = provider.GetRequiredService<IConfiguration>();
            _jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? throw new Exception("JwtSettings is missing.");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
            _userManager = provider.GetRequiredService<UserManager<AppUser>>();
        }

        public string GenerateToken(AppUser appUser)
        {
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName, appUser.UserName!),
            };

            SigningCredentials credential = new(_key, SecurityAlgorithms.HmacSha256);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credential,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            JwtSecurityTokenHandler tokenHandler = new();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}