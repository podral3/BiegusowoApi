namespace BiegusowoApi.Features.Conversations.Dtos;

public record MinimalConversationDto(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    ConversationParticipantDto OtherParticipant,
    MessageDto? LastMessage);