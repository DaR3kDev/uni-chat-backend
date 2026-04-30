using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
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

        // Configuración Mongo
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        services.Configure<MongoSettings>(configuration.GetSection("Mongo"));

        // JWT + Seguridad
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<RefreshTokenSettings>(configuration.GetSection("RefreshToken"));
        services.Configure<Argon2Settings>(configuration.GetSection("Argon2"));

        // Convertir IOptions → instancia directa
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<JwtSettings>>().Value);

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<RefreshTokenSettings>>().Value);

        // MongoClient
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("Mongo ConnectionString is not configured");

            return new MongoClient(settings.ConnectionString);
        });

        // Contexto
        services.AddSingleton<MongoContext>();
        services.AddHostedService<MongoIndexesInitializerService>();

        // Collections
        services.AddSingleton<IMongoCollections, MongoCollections>();

        // Pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Repositories
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();

        // HttpContext
        services.AddHttpContextAccessor();

        // JWT Authentication
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
            
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),

                    NameClaimType = "sub"
                };
            });

        // Security Services
        services.AddSingleton<TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        //// Password hashing
        //services.AddSingleton(sp =>
        //{
        //    var settings = sp.GetRequiredService<IOptions<Argon2Settings>>().Value;
        //    return new Argon2PasswordHasher(settings);
        //});

        //// Otros servicios
        //services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}