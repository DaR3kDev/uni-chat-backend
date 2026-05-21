using System.Text.Json;
using System.Text.Json.Serialization;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class ApiServicesConfiguration
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = false;
            });

        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
    }
}
