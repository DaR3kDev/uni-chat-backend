using FluentValidation;
using MediatR;
using Scalar.AspNetCore;
using System.Reflection;
using uni_chat_backend.API;
using uni_chat_backend.API.Endpoints.Users;
using uni_chat_backend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseHttpsRedirection();

app.MapCreateUser();

app.MapEndpoints();

app.Run();