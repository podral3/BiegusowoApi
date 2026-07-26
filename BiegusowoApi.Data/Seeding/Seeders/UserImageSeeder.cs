using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class UserImageSeeder
{
    public static List<UserImage> Generate(List<User> users)
    {
        var userImages = new List<UserImage>();
        var random = new Random(42);
        int idCounter = 1;

        foreach (var user in users)
        {
            var avatar = new UserImage
            {
                Id = DataSeeder.SeedGuid(idCounter++),
                FileName = $"avatar_{avatarSlug(user)}.webp",
                FileSizeBytes = random.Next(50_000, 2_000_000),
                Type = UserImageType.Avatar,
                CreatedAt = DateTimeOffset.UtcNow
            };
            userImages.Add(avatar);
            user.AvatarImageId = avatar.Id;

            var background = new UserImage
            {
                Id = DataSeeder.SeedGuid(idCounter++),
                FileName = $"background_{avatarSlug(user)}.webp",
                FileSizeBytes = random.Next(100_000, 5_000_000),
                Type = UserImageType.Background,
                CreatedAt = DateTimeOffset.UtcNow
            };
            userImages.Add(background);
            user.BackgroundImageId = background.Id;
        }
        return userImages;

        static string avatarSlug(User u) => u.Id.ToString("N")[..8];
    }
}