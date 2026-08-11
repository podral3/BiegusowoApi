using Ardalis.Result;
using BiegusowoApi.Domain.Dtos.Conversation;
using BiegusowoApi.Helpers;

namespace BiegusowoApi.Domain.Conversations;

public interface IConversationService
{
    Task<CursorPaginatedList<MinimalConversationDto>> GetUserConversationsAsync(
           Guid userId, DateTimeOffset? beforeLastMessageAt, Guid? beforeConversationId, int pageSize);

    Task<Result<ConversationDto>> GetConversationAsync(
        Guid userId, Guid conversationId, DateTimeOffset? beforeCreatedAt, Guid? beforeMessageId, int pageSize);

    Task<Result<ConversationDto>> CreateConversationAsync(Guid buyerId, ConversationRequest request);

    Task<bool> IsUserParticipantInConversationAsync(Guid userId, Guid conversationId);

    Task<bool> ValidateMessageAsync(string message);

    Task<MessageDto> SaveMessageToDbAsync(Guid conversationId, Guid userId, string message);
}
