namespace BiegusowoApi.Features.Conversations.Dtos;

public record ConversationParticipantDto(
    Guid UserId,
    bool IsSelling,
    string DisplayName,
    string? AvatarStorageKey);