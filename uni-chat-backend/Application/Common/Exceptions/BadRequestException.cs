namespace uni_chat_backend.Application.Common.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message)
        : base(message, 400)
    {
        Errors = null;
    }

    public BadRequestException(Dictionary<string, string[]> errors)
        : base("Validation error", 400)
    {
        Errors = errors;
    }

    public object? Errors { get; }
}