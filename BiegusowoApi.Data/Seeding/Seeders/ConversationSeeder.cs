using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class ConversationSeeder
{
    public static List<Conversation> Generate(List<Listing> listings, List<User> users, int maxConversationsPerListing)
    {
        var conversations = new List<Conversation>();
        var random = new Random(42);
        int idCounter = 1;

        foreach (var listing in listings)
        {
            if (random.Next(100) < 30) continue;

            int conversationCount = random.Next(maxConversationsPerListing + 1);
            for (int i = 0; i < conversationCount; i++)
            {
                User buyer = users[random.Next(users.Count)];
                User seller = users.FirstOrDefault(u => u.Id == listing.UserId)!;

                while (buyer.Id == seller.Id)
                {
                    buyer = users[random.Next(users.Count)];
                }
                var conversation = new Conversation
                {
                    Id = DataSeeder.SeedGuid(idCounter++),
                    Listing = listing,
                    Buyer = buyer,
                    Seller = seller,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-random.Next(10))
                };
                conversations.Add(conversation);
            }
        }
        return conversations;
    }
}
