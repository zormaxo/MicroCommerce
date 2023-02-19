using System.Security.Claims;

namespace Basket.API.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _context;

    public IdentityService(IHttpContextAccessor context)
    { _context = context ?? throw new ArgumentNullException(nameof(context)); }

    public string GetUserName()
    { return _context.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value; }
}

