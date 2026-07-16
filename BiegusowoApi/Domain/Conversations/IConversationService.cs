using BiegusowoApi.Domain.Dtos.Conversation;
using BiegusowoApi.Helpers;

namespace BiegusowoApi.Domain.Conversations;

public interface IConversationService
{
    /// <summary>
    /// Returns user conversations in latest order.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<PaginatedList<MinimalConversationDto>> GetUserConversationsAsync(string userId, int page, int pageSize);
    public Task<PaginatedList<MessageDto>> GetConversationMessagesAsync(string conversationId, int page, int pageSize);
    public Task<MessageDto?> SaveMessageToDbAsync(string conversationId, string SenderId, string message);
    public Task<bool> ValidateMessageAsync(string message);
    public Task<bool> IsUserParticipantInConversationAsync(string userId, string conversationId);
}
