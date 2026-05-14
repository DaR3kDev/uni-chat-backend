namespace uni_chat_backend.Application.Common.Exceptions;

public class ConflictException(string message) : AppException(message, 409)
{
}