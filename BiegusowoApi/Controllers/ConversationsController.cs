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
    [EndpointDescription("Get a paginated list of user conversations. Does not include messages.")]
    public async Task<ActionResult<PaginatedList<MinimalConversationDto>>> GetConversations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok();
    }

    [HttpGet("{conversationId}")]
    [Authorize]
    public async Task<ActionResult<ConversationDto>> GetMessages(
        [FromRoute] string conversationId,
        [FromQuery] int skip = 0,
        [FromQuery] int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] ConversationRequest request)
    {
        throw new NotImplementedException();
    }
}
