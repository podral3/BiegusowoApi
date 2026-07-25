using BiegusowoApi.Data;
using BiegusowoApi.Domain.Conversations;
using BiegusowoApi.Domain.Dtos.Conversation;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BiegusowoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PaginatedList<MinimalConversationDto>>> GetConversations(
        [FromQuery] DateTimeOffset? beforeLastMessageAt,
        [FromQuery] Guid? beforeConversationId,
        [FromQuery] int pageSize = 10)
    {
        throw new NotImplementedException();    
    }

    [HttpGet("{conversationId:guid}")]
    [Authorize]
    public async Task<ActionResult<ConversationDto>> GetMessages(
        [FromRoute] Guid conversationId,
        [FromQuery] DateTimeOffset? beforeCreatedAt,
        [FromQuery] Guid? beforeMessageId, 
        [FromQuery] int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Authorize]
    [EndpointDescription("Create a new conversation regarding a specific listing.")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] ConversationRequest request)
    {
        throw new NotImplementedException();
    }
}
