using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using BiegusowoApi.Data;
using BiegusowoApi.Features.Conversations.Dtos;
using BiegusowoApi.Shared.Helpers;
using BiegusowoApi.Shared.Helpers.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BiegusowoApi.Features.Conversations;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController(IConversationService conversationService) : ControllerBase
{
    private readonly IConversationService _conversationService = conversationService;
    private Guid IdentityId => User.GetUserId();

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<CursorPaginatedList<MinimalConversationDto>>> GetConversations(
        [FromQuery] DateTimeOffset? beforeLastMessageAt,
        [FromQuery] Guid? beforeConversationId,
        [FromQuery] int pageSize = 10)
    {
        var result = await _conversationService.GetUserConversationsAsync(IdentityId, beforeLastMessageAt, beforeConversationId, pageSize);
        return Ok(result);

    }

    [HttpGet("{conversationId:guid}")]
    [Authorize]
    public async Task<ActionResult<ConversationDto>> GetMessages(
        [FromRoute] Guid conversationId,
        [FromQuery] DateTimeOffset? beforeCreatedAt,
        [FromQuery] Guid? beforeMessageId,
        [FromQuery] int pageSize = 10)
    {
        Result<ConversationDto> result = await _conversationService
            .GetConversationAsync(IdentityId, conversationId, beforeCreatedAt, beforeMessageId, pageSize);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [Authorize]
    [EndpointDescription("Create a new conversation regarding a specific listing.")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] ConversationRequest request)
    {
        Result<ConversationDto> result = await _conversationService.CreateConversationAsync(IdentityId, request);
        return result.ToActionResult(this);
    }
}
