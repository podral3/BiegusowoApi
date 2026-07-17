using BiegusowoApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class MessageSeeder
{
    public static List<Message> Generate(List<Conversation> conversations, int maxMessagesPerConversation)
    {
        var messages = new List<Message>();
        var random = new Random(42);
        foreach (var conversation in conversations)
        {
            int messageCount = random.Next(1, maxMessagesPerConversation + 1);
            var currentTimestamp = conversation.CreatedAt;

            for (int i = 0; i < messageCount; i++)
            {
                // Simulate realistic reply gaps (e.g., between 5 minutes and 4 hours)
                int minutesToAdd = random.Next(5, 240);
                currentTimestamp = currentTimestamp.AddMinutes(minutesToAdd);

                var message = new Message
                {
                    Conversation = conversation,
                    SenderId = random.Next(2) == 0 ? conversation.BuyerId : conversation.SellerId,
                    Body = $"Message {i + 1} in conversation {conversation.Id}",
                    CreatedAt = currentTimestamp
                };
                if ( i == 0 ) message.Sender = conversation.Buyer;

                messages.Add(message);
            }
        }
        return [.. messages.OrderBy(m => m.CreatedAt)];
    }
}
