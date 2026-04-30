using System.Net;
using System.Text.Json;
using uni_chat_backend.API.Exceptions;
using uni_chat_backend.API.Responses;

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
        catch (BadRequestException ex)
        {
            await Handle(context, HttpStatusCode.BadRequest, ex);
        }
        catch (UnauthorizedException ex)
        {
            await Handle(context, HttpStatusCode.Unauthorized, ex);
        }
        catch (NotFoundException ex)
        {
            await Handle(context, HttpStatusCode.NotFound, ex);
        }
        catch (Exception ex)
        {
            await Handle(context, HttpStatusCode.InternalServerError, ex);
        }
    }

    private static async Task Handle(HttpContext context, HttpStatusCode status, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var response = new ApiResponse
        {
            StatusCode = (int)status,
            Message = ex.Message
        };

        if (ex is BadRequestException br && br.Errors is not null)
        {
            response.Errors = br.Errors;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}