using BiegusowoApi.Domain.Dtos.Conversation;
using BiegusowoApi.Helpers;

namespace BiegusowoApi.Domain.Conversations;

public interface IConversationService
{
    Task<CursorPaginatedList<MinimalConversationDto>> GetUserConversationsAsync(
           Guid identityId, DateTimeOffset? beforeLastMessageAt, Guid? beforeConversationId, int pageSize);

    Task<ConversationDto?> GetConversationAsync(
        Guid identityId, Guid conversationId, DateTimeOffset? beforeCreatedAt, Guid? beforeMessageId, int pageSize);

    Task<ConversationDto?> CreateConversationAsync(Guid identityId, ConversationRequest request);

    Task<bool> IsUserParticipantInConversationAsync(Guid identityId, Guid conversationId);

    Task<bool> ValidateMessageAsync(string message);

    Task<MessageDto?> SaveMessageToDbAsync(Guid conversationId, Guid identityId, string message);

    // resolves who the *other* participant is in a given conversation, for hub group bookkeeping etc.
    Task<Guid?> GetLocalUserIdAsync(Guid identityId);
}
