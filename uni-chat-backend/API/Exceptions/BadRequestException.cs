namespace uni_chat_backend.API.Exceptions;

public class BadRequestException(List<string> errors) : Exception("Validation failed")
{
    public List<string> Errors { get; } = errors;
}