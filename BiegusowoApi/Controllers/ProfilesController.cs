using BiegusowoApi.Domain.ProfilePage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfilesController : ControllerBase
{
    [HttpGet("me")]
    public Task<ActionResult<ProfilePageResponse>> GetMyProfile() 
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public Task<ActionResult<ProfilePageResponse>> GetProfile(string id) 
    {
        throw new NotImplementedException();
    }
}
