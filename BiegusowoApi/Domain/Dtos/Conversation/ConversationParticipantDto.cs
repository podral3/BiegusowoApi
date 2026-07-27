namespace BiegusowoApi.Domain.Dtos.Conversation;

public record ConversationParticipantDto(
    Guid UserId,
    string DisplayName,
    string? AvatarFileName);