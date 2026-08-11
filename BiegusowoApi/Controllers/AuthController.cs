using Ardalis.Result.AspNetCore;
using BiegusowoApi.Domain.Account;
using BiegusowoApi.Domain.Dtos.User;
using BiegusowoApi.Domain.Register;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using static BiegusowoApi.Domain.Dtos.Auth.AuthDtos;

namespace BiegusowoApi.Controllers;

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
        [FromBody] ResendVerificationEmailRequest request,
        CancellationToken ct)
    {
        var result = await _accountService.ResendVerificationEmailAsync(request.Email, ct);
        return result.ToActionResult(this);
    }

    [HttpPost("password-reset")]
    [EnableRateLimiting("email-actions")]
    public async Task<ActionResult> RequestPasswordReset(
        [FromBody] PasswordResetRequest request,
        CancellationToken ct)
    {
        var result = await _accountService.RequestPasswordResetAsync(request.Email, ct);
        return result.ToActionResult(this);
    }
}
