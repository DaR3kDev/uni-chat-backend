using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using uni_chat_backend.Infrastructure.Configuration;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class AuthenticationInjection
{
    public static void AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration
                              .GetSection("Jwt")
                              .Get<JwtSettings>()
                          ?? throw new InvalidOperationException(
                              "Jwt settings missing"
                          );

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,

                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings.Key)
                            ),

                        NameClaimType =
                            ClaimTypes.NameIdentifier
                    };

                // =====================================================
                // SIGNALR
                // =====================================================
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"].FirstOrDefault();

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/messages/chat"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });
    }
}