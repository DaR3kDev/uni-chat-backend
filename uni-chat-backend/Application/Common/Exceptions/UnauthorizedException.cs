namespace uni_chat_backend.Application.Common.Exceptions;
public class UnauthorizedException(string message) : AppException(message, 401)
{
}
