using Serilog;
using uni_chat_backend.API.Configuration.DependencyInjection;
using uni_chat_backend.API.Extensions;
using uni_chat_backend.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "uni-chat-backend")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiServices();
builder.Services.AddCorsConfiguration();
builder.Services.AddMediatorConfiguration();
builder.Services.AddValidationConfiguration();
builder.Host.AddWolverineConfiguration(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseApiDocs();

app.UseSerilogRequestLogging();

app.UseApiMiddleware();

app.Run();