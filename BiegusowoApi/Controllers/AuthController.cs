using BiegusowoApi.Domain.Dtos.User;
using BiegusowoApi.Domain.Register;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IRegisterService registerService
    ) :ControllerBase
{
    private readonly IRegisterService _registerService = registerService;

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResult>> Register(RegisterRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _registerService.RegisterAsync(request, ct);
            return Ok(result);
        }
        catch (RegistrationFailedException ex)
        {
            return Problem(ex.Message, statusCode: StatusCodes.Status502BadGateway);
        }
    }
}
