using DotNetEnv;
using Serilog;
using Serilog.Exceptions;
using uni_chat_backend.API.Configuration.DependencyInjection;
using uni_chat_backend.API.Configuration.Middleware;
using uni_chat_backend.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithExceptionDetails()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] " +
        "[RequestId: {RequestId}] " +
        "[Machine: {MachineName}] " +
        "[Thread: {ThreadId}] " +
        "{Message:lj}{NewLine}{Exception}"
    )
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
