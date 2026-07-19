namespace BiegusowoApi.Domain.Conversations;

public record ConversationRequest(string ListingId,
    string FirstMessage,
    DateTimeOffset SentAt);

