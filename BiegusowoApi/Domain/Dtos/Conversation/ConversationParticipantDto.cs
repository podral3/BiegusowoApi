namespace BiegusowoApi.Domain.Dtos.Conversation;

public record ConversationParticipantDto(
    Guid UserId,
    bool IsSelling,
    string DisplayName,
    string? AvatarStorageKey);