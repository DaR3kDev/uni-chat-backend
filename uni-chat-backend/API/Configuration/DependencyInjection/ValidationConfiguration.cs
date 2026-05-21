using System.Reflection;
using FluentValidation;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class ValidationConfiguration
{
    public static void AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
