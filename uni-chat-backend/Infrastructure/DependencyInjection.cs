using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Security.Claims;
using System.Text;
using uni_chat_backend.Application.Behaviors;
using uni_chat_backend.Infrastructure.Configuration;
using uni_chat_backend.Infrastructure.Persistence;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Persistence.Indexes.Initialization;
using uni_chat_backend.Infrastructure.Repositories;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // =========================
        // MongoDB
        // =========================
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.Configure<MongoSettings>(configuration.GetSection("Mongo"));

        // =========================
        // JWT / Refresh Token 
        // =========================
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<RefreshTokenSettings>(configuration.GetSection("RefreshToken"));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<JwtSettings>>().Value);

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<RefreshTokenSettings>>().Value);

        // =========================
        // Mongo Client
        // =========================
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("Mongo ConnectionString is not configured");

            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton<MongoContext>();
        services.AddHostedService<MongoIndexesInitializerService>();

        services.AddSingleton<IMongoCollections, MongoCollections>();

        // =========================
        // MediatR Pipeline
        // =========================
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // =========================
        // Repositories
        // =========================
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();

        services.AddHttpContextAccessor();

        // =========================
        // JWT AUTH (CORRECTO PARA SIGNALR)
        // =========================
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt settings missing");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)
                    ),

                    NameClaimType = ClaimTypes.NameIdentifier
                };

                // =========================
                // SIGNALR
                // =========================
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"].FirstOrDefault();

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        // =========================
        // SECURITY SERVICES
        // =========================
        services.AddSingleton<TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}