using System.Security.Claims;

namespace ProjectBoard.API.Http;

public class HttpExecutionContext : IExecutionContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpExecutionContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public CurrentUser? GetCurrentIdentity()
    {        
        string? userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);        
        return new CurrentUser() { UserId = userId };        
    }      
}
