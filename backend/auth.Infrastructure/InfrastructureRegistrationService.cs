using auth.Application.Interfaces;
using auth.Infrastructure.Persistence;
using auth.Infrastructure.Persistence.Seeder;
using auth.Infrastructure.Services;
using auth.Infrastructure.settings;
using Auth.Infrastructure.Persistence.Seeder;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace auth.Infrastructure;

public static class InfrastructureRegistrationService
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IAuthDbContext, AuthDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            if (sp.GetRequiredService<IHostEnvironment>().IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IClientInfoProvider, ClientInfoProvider>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<RoleSeeder>();
        services.AddScoped<PermissionSeeder>();
        services.AddScoped<UserSeeder>();
        services.AddScoped<DatabaseSeeder>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        })
        .AddJwtBearer("Bearer", options =>
        {
            var jwtSettings = configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            if (jwtSettings is null)
                throw new InvalidOperationException("JWT settings are not configured properly.");

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)),

                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext
                        .RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("JWT");

                    logger.LogWarning(
                        "JWT Authentication failed: {Message} | Path: {Path}",
                        context.Exception.Message,
                        context.Request.Path);

                    return Task.CompletedTask;
                },

                OnChallenge = context =>
                {
                    var logger = context.HttpContext
                        .RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("JWT");

                    logger.LogWarning(
                        "Unauthorized request to {Path} | IP: {IP}",
                        context.Request.Path,
                        context.HttpContext.Connection.RemoteIpAddress);

                    return Task.CompletedTask;
                },

                OnTokenValidated = async context =>
                {
                    var logger = context.HttpContext
                        .RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("JWT");

                    var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var permVersionClaim = context.Principal?.FindFirst("perm_version")?.Value;

                    if (!int.TryParse(userIdClaim, out var userId)
                        || !int.TryParse(permVersionClaim, out var permVersion))
                    {
                        context.Fail("Invalid token claims.");
                        return;
                    }

                    var db = context.HttpContext.RequestServices.GetRequiredService<IAuthDbContext>();
                    var currentVersion = await db.Users
                        .AsNoTracking()
                        .Where(u => u.Id == userId)
                        .Select(u => u.PermissionsVersion)
                        .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

                    if (currentVersion != permVersion)
                    {
                        logger.LogWarning(
                            "Token permissions outdated for UserId: {UserId}", userId);
                        context.Fail("Token permissions are outdated.");
                        return;
                    }

                    logger.LogInformation(
                        "Token validated successfully for UserId: {UserId}", userId);
                }
            };
        });

        return services;
    }
}
