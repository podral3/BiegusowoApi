using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Domain.Dtos.Conversation;

public record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string Body,
    MessageStatus MessageStatus,
    DateTimeOffset CreatedAt);
