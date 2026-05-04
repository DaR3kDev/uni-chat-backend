namespace uni_chat_backend.Application.Common.Exceptions;

public class NotFoundException(string message) : AppException(message, 404)
{
}
