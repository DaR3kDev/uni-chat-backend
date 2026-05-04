namespace uni_chat_backend.Application.Common.Exceptions;
public class ForbiddenException(string message) : AppException(message, 403)
{
}
