namespace BiegusowoApi.Domain.Dtos.Conversation;

public record MinimalConversationDto(
    Guid Id,
    Guid OtherPersonId,
    string OtherPersonName,
    Guid ListingId,
    string ListingTitle,
    bool AreYouSeller,
    string LastMessagePreview,
    DateTimeOffset LastMessageAt);