using System.Net;
using System.Text.Json;
using uni_chat_backend.API.Responses;
using uni_chat_backend.Application.Common.Exceptions;

namespace uni_chat_backend.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            await Handle(context, ex.StatusCode, ex);
        }
        catch (Exception ex)
        {
            await Handle(context, (int)HttpStatusCode.InternalServerError, ex);
        }
    }

    private static async Task Handle(HttpContext context, int status, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;

        var response = new ApiResponse
        {
            StatusCode = status,
            Message = ex.Message
        };

        if (ex is BadRequestException br && br.Errors is not null)
            response.Errors = br.Errors;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response)
        );
    }
}
