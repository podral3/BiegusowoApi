namespace BiegusowoApi.Features.Conversations.Dtos;

public record ConversationRequest(Guid ListingId,
    string FirstMessage,
    DateTimeOffset SentAt);

