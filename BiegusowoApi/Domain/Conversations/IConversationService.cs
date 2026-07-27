using BiegusowoApi.Domain.Dtos.Conversation;
using BiegusowoApi.Helpers;

namespace BiegusowoApi.Domain.Conversations;

public interface IConversationService
{
    Task<CursorPaginatedList<MinimalConversationDto>> GetUserConversationsAsync(
           Guid identityId, DateTimeOffset? beforeLastMessageAt, Guid? beforeConversationId, int pageSize);

    Task<Result<ConversationDto?>> GetConversationAsync(
        Guid identityId, Guid conversationId, DateTimeOffset? beforeCreatedAt, Guid? beforeMessageId, int pageSize);

    Task<Result<ConversationDto?>> CreateConversationAsync(Guid identityId, ConversationRequest request);

    Task<bool> IsUserParticipantInConversationAsync(Guid identityId, Guid conversationId);

    Task<bool> ValidateMessageAsync(string message);

    Task<MessageDto?> SaveMessageToDbAsync(Guid conversationId, Guid identityId, string message);
}
