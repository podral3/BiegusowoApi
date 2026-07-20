namespace BiegusowoApi.Domain.Dtos.Conversation;

public record MessageDto(
    Guid ConversationId,
    Guid SenderId,
    string Message,
    DateTimeOffset SentAt);
