using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Types;
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
        int idCounter = 1;

        foreach (var conversation in conversations)
        {
            int messageCount = random.Next(1, maxMessagesPerConversation + 1);
            var currentTimestamp = conversation.CreatedAt;

            for (int i = 0; i < messageCount; i++)
            {
                int minutesToAdd = random.Next(5, 240);
                currentTimestamp = currentTimestamp.AddMinutes(minutesToAdd);

                var message = new Message
                {
                    Id = DataSeeder.SeedGuid(idCounter++),
                    Conversation = conversation,
                    SenderId = random.Next(2) == 0 ? conversation.BuyerId : conversation.SellerId,
                    Body = $"Message {i + 1} in conversation {conversation.Id}",
                    CreatedAt = currentTimestamp,
                    MessageStatus = MessageStatus.Read
                };
                if (i == messageCount - 1)
                {
                    message.MessageStatus = random.Next(2) == 0 ? MessageStatus.Read : MessageStatus.Unread;
                }

                messages.Add(message);
            }
        }
        return [.. messages.OrderBy(m => m.CreatedAt)];
    }
}
