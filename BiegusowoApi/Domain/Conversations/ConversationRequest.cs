namespace BiegusowoApi.Domain.Conversations;

public class ConversationRequest
{
    public string ListingId { get; set; }
    public string ReceiverId { get; set; }
    public string FirstMessage { get; set; }
    public DateTimeOffset SentAt { get; set; }
}
