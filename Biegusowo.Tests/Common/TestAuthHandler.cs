using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Biegusowo.Tests.Common;

public class TestAuthContext
{
    public int UserId { get; set; }
    public string? Email { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestAuthContext _context;

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TestAuthContext context)
        : base(options, logger, encoder)
    {
        _context = context;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _context.UserId.ToString()),
            new Claim("sub", _context.UserId.ToString()),
        };

        if (_context.Email is not null)
            claims.Add(new Claim(ClaimTypes.Email, _context.Email));

        claims.AddRange(_context.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
