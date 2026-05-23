namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class CorsConfiguration
{
    private static readonly string[] AllowedOrigins =
    [
        "http://localhost:5173",
        "http://localhost:3000",
        "http://localhost:8080",
        "https://uni-chat-five.vercel.app/"
    ];

    public static void AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .WithOrigins(AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}
