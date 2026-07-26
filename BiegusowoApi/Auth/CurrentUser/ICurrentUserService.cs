using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Auth.CurrentUser;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Task<User> GetUserAsync();
}