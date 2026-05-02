namespace uni_chat_backend.API.Exceptions;

public class BadRequestException : Exception
{
    public List<string>? Errors { get; }

    public BadRequestException(string message): base(message)
    {
    }

    public BadRequestException(List<string> errors): base("Validation failed")
    {
        Errors = errors;
    }
}