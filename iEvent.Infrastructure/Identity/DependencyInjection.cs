using iEvent.Application.Interfaces.Services;
using iEvent.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace iEvent.Infrastructure.Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
        {
            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection("Jwt");
            services.Configure<JwtSettings>(section);

            var settings = section.Get<JwtSettings>() ?? new JwtSettings();

            if (string.IsNullOrWhiteSpace(settings.Key)
                || string.IsNullOrWhiteSpace(settings.Issuer)
                || string.IsNullOrWhiteSpace(settings.Audience))
            {
                throw new InvalidOperationException(
                    "Jwt settings are missing. Configure Jwt:Issuer, Jwt:Audience, and Jwt:Key.");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = settings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = settings.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuerSigningKey = true,
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsJsonAsync(new
                            {
                                StatusCode = 401,
                                Message = "Authentication required. Please provide a valid JWT token."
                            });
                        },

                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsJsonAsync(new
                            {
                                StatusCode = 403,
                                Message = "You do not have permission to access this resource."
                            });
                        }
                    };
                });

            return services;
        }
    }
}
