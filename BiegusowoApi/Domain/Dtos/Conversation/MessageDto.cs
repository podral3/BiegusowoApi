namespace BiegusowoApi.Domain.Dtos.Conversation;

public record MessageDto(
    string ConversationId,
    string SenderId,
    string Message,
    DateTimeOffset SentAt);
