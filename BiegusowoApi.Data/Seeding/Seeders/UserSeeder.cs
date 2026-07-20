using BiegusowoApi.Data.Models;
using Bogus;
using Bogus.Extensions;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class UserSeeder
{
    public static List<User> Generate(int count, List<Voivodeship> voivodeships)
    {
        var userFaker = new Faker<User>()
            .UseSeed(42)
            .RuleFor(u => u.IdentityId, f => f.Random.Guid().ToString())
            .RuleFor(u => u.DisplayName, f => f.Internet.UserName())
            .RuleFor(u => u.Bio, f => f.Lorem.Paragraph().OrNull(f, 0.2f))
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber().OrNull(f, 0.2f))
            .RuleFor(u => u.City, f => f.Address.City())
            .RuleFor(u => u.VoivodeshipId, f => f.PickRandom(voivodeships).Id);

        return userFaker.Generate(count);
    }
}
