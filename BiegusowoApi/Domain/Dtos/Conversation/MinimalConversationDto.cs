namespace BiegusowoApi.Domain.Dtos.Conversation;

public record MinimalConversationDto(
    string Id,
    string OtherPersonId,
    string OtherPersonName,
    string ListingId,
    string ListingTitle,
    bool AreYouSeller);