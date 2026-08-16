using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Seeding.Seeders;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Data.Seeding;

public class DataSeeder(ApplicationDbContext dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task Seed()
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync();

            await InlineDataSeeder.Seed(_dbContext);

            var allBlobs = new List<Blob>();

            var (users, userBlobs) =
                UserSeeder.Generate(100, InlineDataSeeder.Voivodeships);

            allBlobs.AddRange(userBlobs);

            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            var (listings, listingBlobs) =
                ListingSeeder.Generate(
                    100,
                    InlineDataSeeder.Breeds,
                    users,
                    InlineDataSeeder.Voivodeships);

            allBlobs.AddRange(listingBlobs);

            await _dbContext.Listings.AddRangeAsync(listings);
            await _dbContext.SaveChangesAsync();

            var conversations =
                ConversationSeeder.Generate(listings, users, 5);

            await _dbContext.Conversations.AddRangeAsync(conversations);
            await _dbContext.SaveChangesAsync();

            var messages =
                MessageSeeder.Generate(conversations, 20);

            await _dbContext.Messages.AddRangeAsync(messages);
            await _dbContext.SaveChangesAsync();

            var (articles, articleBlobs) = ArticleSeeder.Generate(10);

            allBlobs.AddRange(articleBlobs);

            await _dbContext.Articles.AddRangeAsync(articles);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Blobs.AddRangeAsync(allBlobs);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        });
    }

    public static Guid SeedGuid(int number) =>
        new($"00000000-0000-0000-0000-{number:D12}");
}