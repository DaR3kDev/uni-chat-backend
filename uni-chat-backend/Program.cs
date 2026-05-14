using uni_chat_backend.API.Configuration.DependencyInjection;
using uni_chat_backend.API.Configuration.Middleware;
using uni_chat_backend.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiServices();
builder.Services.AddCorsConfiguration();
builder.Services.AddMediatorConfiguration();
builder.Services.AddValidationConfiguration();

builder.Host.AddWolverineConfiguration(builder.Configuration);

var app = builder.Build();

app.UseApiMiddleware();

app.MapApiEndpoints();

app.Run();