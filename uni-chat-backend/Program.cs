using DotNetEnv;
using Serilog;
using Serilog.Exceptions;
using uni_chat_backend.API.Configuration.DependencyInjection;
using uni_chat_backend.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithExceptionDetails()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:80")
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
