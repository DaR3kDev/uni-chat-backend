using FluentValidation;
using Scalar.AspNetCore;
using System.Reflection;
using System.Text.Json.Serialization;
using uni_chat_backend.API.Extensions;
using uni_chat_backend.Features.Messages.SendMessage;
using uni_chat_backend.Infrastructure.DependencyInjection;
using uni_chat_backend.Infrastructure.SignalR;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(new Uri(builder.Configuration["RabbitMQ:ConnectionString"]!))
        .AutoProvision();

    opts.PublishMessage<SendMessageEvent>()
        .ToRabbitQueue("messages.send");

    opts.ListenToRabbitQueue("messages.send");
});

builder.Services.AddSignalRServices();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/messages/chat");

app.MapEndpoints();

app.UseCustomMiddlewares();

app.Run();