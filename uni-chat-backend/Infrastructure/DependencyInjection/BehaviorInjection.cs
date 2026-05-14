using MediatR;
using uni_chat_backend.Application.Common.Behaviors;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class BehaviorInjection
{
    public static void AddApplicationBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}