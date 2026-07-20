namespace BiegusowoApi.Domain.Conversations;

public record ConversationRequest(Guid ListingId,
    string FirstMessage,
    DateTimeOffset SentAt);

