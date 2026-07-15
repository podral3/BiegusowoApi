using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Data.Models;

public class Message
{
    public Guid Id { get; set; }
    
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public string Body { get;set; }
    public MessageStatus MessageStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
