using System.Security.Claims;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Infrastructure.Security;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}