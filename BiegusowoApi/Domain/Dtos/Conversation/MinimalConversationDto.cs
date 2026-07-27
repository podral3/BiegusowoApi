namespace BiegusowoApi.Domain.Dtos.Conversation;

public record MinimalConversationDto(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    ConversationParticipantDto OtherParticipant,
    MessageDto? LastMessage);