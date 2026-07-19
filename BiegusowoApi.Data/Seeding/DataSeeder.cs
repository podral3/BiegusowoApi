using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Seeding.Seeders;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Data.Seeding;

public class DataSeeder(ApplicationDbContext dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    public async Task Seed()
    {
        await _dbContext.Database.MigrateAsync();

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await InlineDataSeeder.Seed(_dbContext);

            List<User> users = UserSeeder.Generate(100, InlineDataSeeder.Voivodeships);
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            List<Listing> listings = ListingSeeder.Generate(100, InlineDataSeeder.Breeds, users, InlineDataSeeder.Voivodeships);
            await _dbContext.Listings.AddRangeAsync(listings);
            await _dbContext.SaveChangesAsync();

            List<ListingPhoto> listingPhotos = ListingPhotoSeeder.Generate(listings, 10);
            await _dbContext.ListingPhotos.AddRangeAsync(listingPhotos);
            await _dbContext.SaveChangesAsync();

            List<Conversation> conversations = ConversationSeeder.Generate(listings, users, 5);
            await _dbContext.Conversations.AddRangeAsync(conversations);
            await _dbContext.SaveChangesAsync();

            List<Message> messages = MessageSeeder.Generate(conversations, 20);
            await _dbContext.Messages.AddRangeAsync(messages);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
