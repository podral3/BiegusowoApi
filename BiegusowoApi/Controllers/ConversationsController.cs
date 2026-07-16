using BiegusowoApi.Data;
using BiegusowoApi.Domain.Conversations;
using BiegusowoApi.Domain.Dtos.Conversation;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PaginatedList<MinimalConversationDto>>> GetConversations()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ConversationDto>> GetConversation(string id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ConversationDto>> GetOrCreateConversation([FromBody] ConversationRequest request)
    {
        throw new NotImplementedException();
    }
}
