using BiegusowoApi.Domain.Dtos.User;

namespace BiegusowoApi.Domain.Register;

public interface IRegisterService
{
    public Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    public Task DeleteKeycloakUserAsync(Guid userId, CancellationToken ct);
    
    }
