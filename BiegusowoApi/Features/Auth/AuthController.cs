using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BiegusowoApi.Features.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAccountService registerService
    ) :ControllerBase
{
    private readonly IAccountService _accountService = registerService;

    [HttpPost("register")]
    [EnableRateLimiting("email-actions")]
    public async Task<ActionResult<RegisterResult>> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await _accountService.RegisterAsync(request, ct);
        return result.ToActionResult(this);
    }

    [HttpPost("resend-verification")]
    [EnableRateLimiting("email-actions")]
    public async Task<ActionResult> ResendVerificationEmail(
        [FromBody] string email,
        CancellationToken ct)
    {
        var result = await _accountService.ResendVerificationEmailAsync(email, ct);
        return result.ToActionResult(this);
    }

    [HttpPost("password-reset")]
    [EnableRateLimiting("email-actions")]
    public async Task<ActionResult> RequestPasswordReset(
        [FromBody] string email,
        CancellationToken ct)
    {
        var result = await _accountService.RequestPasswordResetAsync(email, ct);
        return result.ToActionResult(this);
    }
}
