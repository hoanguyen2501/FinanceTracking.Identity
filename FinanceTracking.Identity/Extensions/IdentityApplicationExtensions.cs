using System.Text;
using FinanceTracking.Identity.Data;
using FinanceTracking.Identity.Entities;
using FinanceTracking.Identity.Helpers;
using FinanceTracking.Identity.Mappers;
using FinanceTracking.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracking.Identity.Extensions
{
    public static class IdentityApplicationExtensions
    {
        public static IServiceCollection AddApplicationExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityServices(configuration);
            services.AddAutoMapper(typeof(DefaultMapperProfile));

            return services;
        }

        private static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddDbContext<IdentityDbContext>((sp, options) => options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Doesn't have config for DefaultConnection"),
                b => b.MigrationsAssembly("FinanceTracking.Identity")
                        .EnableRetryOnFailure(10)
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .EnableServiceProviderCaching());

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>();

            services.AddJWTServices(configuration);
            return services;
        }

        private static IServiceCollection AddJWTServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                JwtSettings jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                    ?? throw new Exception($"{nameof(JwtSettings)} configuration is missing.");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
                };
            });

            services.AddScoped<ITokenService, TokenService>();

            services.AddAuthorization();

            return services;
        }
    }
}