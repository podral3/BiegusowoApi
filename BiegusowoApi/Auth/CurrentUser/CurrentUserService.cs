using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Auth.CurrentUser;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _dbContext;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public Guid UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(sub, out var userId))
                throw new UnauthorizedAccessException();

            return userId;
        }
    }

    public async Task<User> GetUserAsync()
    {
        var user = await _dbContext.Users.FindAsync(UserId);

        return user ?? throw new UnauthorizedAccessException();
    }
}