using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class InlineDataSeeder
{
    public static async Task Seed(ApplicationDbContext dbContext)
    {
        if (!dbContext.Species.Any())
        {
            await dbContext.Species.AddRangeAsync(Species);
            await dbContext.SaveChangesAsync();
        }
        if (!dbContext.Voivodeships.Any())
        {
            await dbContext.Voivodeships.AddRangeAsync(Voivodeships);
            await dbContext.SaveChangesAsync();
        }
        if (!dbContext.Breeds.Any())
        {
            await dbContext.Breeds.AddRangeAsync(Breeds);
            await dbContext.SaveChangesAsync();
        }
    }

    public static List<Species> Species =>
    [
        new() { Id = 1, Name = "Kury",       Slug = "kury",       SortOrder = 1,  IsActive = true },
        new() { Id = 2, Name = "Kaczki",     Slug = "kaczki",     SortOrder = 2,  IsActive = true },
        new() { Id = 3, Name = "Gęsi",       Slug = "gesi",       SortOrder = 3,  IsActive = true },
        new() { Id = 4, Name = "Indyki",     Slug = "indyki",     SortOrder = 4,  IsActive = true },
        new() { Id = 5, Name = "Perliczki",  Slug = "perliczki",  SortOrder = 5,  IsActive = true },
        new() { Id = 6, Name = "Przepiórki", Slug = "przepiorki", SortOrder = 6,  IsActive = true },
        new() { Id = 7, Name = "Bażanty",    Slug = "bazanty",    SortOrder = 7,  IsActive = true },
        new() { Id = 8, Name = "Gołębie",    Slug = "golebie",    SortOrder = 8,  IsActive = true },
        new() { Id = 9, Name = "Inne",       Slug = "inne",       SortOrder = 99, IsActive = true },
    ];

    public static List<Voivodeship> Voivodeships =>
    [
        new() { Id = 2,  Name = "dolnośląskie" },
        new() { Id = 4,  Name = "kujawsko-pomorskie" },
        new() { Id = 6,  Name = "lubelskie" },
        new() { Id = 8,  Name = "lubuskie" },
        new() { Id = 10, Name = "łódzkie" },
        new() { Id = 12, Name = "małopolskie" },
        new() { Id = 14, Name = "mazowieckie" },
        new() { Id = 16, Name = "opolskie" },
        new() { Id = 18, Name = "podkarpackie" },
        new() { Id = 20, Name = "podlaskie" },
        new() { Id = 22, Name = "pomorskie" },
        new() { Id = 24, Name = "śląskie" },
        new() { Id = 26, Name = "świętokrzyskie" },
        new() { Id = 28, Name = "warmińsko-mazurskie" },
        new() { Id = 30, Name = "wielkopolskie" },
        new() { Id = 32, Name = "zachodniopomorskie" },
    ];

    public static List<Breed> Breeds =>
    [
        // Kury (1)
        new() { Id = 1,  SpeciesId = 1, Name = "Rosa",                      Slug = "rosa",                      IsActive = true },
        new() { Id = 2,  SpeciesId = 1, Name = "Leghorn",                   Slug = "leghorn",                   IsActive = true },
        new() { Id = 3,  SpeciesId = 1, Name = "Plymouth Rock",             Slug = "plymouth-rock",             IsActive = true },
        new() { Id = 4,  SpeciesId = 1, Name = "Sussex",                    Slug = "sussex",                    IsActive = true },
        new() { Id = 5,  SpeciesId = 1, Name = "Wyandotte",                 Slug = "wyandotte",                 IsActive = true },
        new() { Id = 6,  SpeciesId = 1, Name = "Zielononóżka kuropatwiana", Slug = "zielononozka-kuropatwiana", IsActive = true },
        new() { Id = 7,  SpeciesId = 1, Name = "Kochin",                    Slug = "kochin",                    IsActive = true },
        new() { Id = 8,  SpeciesId = 1, Name = "Orpington",                 Slug = "orpington",                 IsActive = true },
        new() { Id = 9,  SpeciesId = 1, Name = "Rhode Island Red",          Slug = "rhode-island-red",          IsActive = true },
        new() { Id = 10, SpeciesId = 1, Name = "Cochin bantam",             Slug = "cochin-bantam",             IsActive = true },
        new() { Id = 11, SpeciesId = 1, Name = "New Hampshire",             Slug = "new-hampshire",             IsActive = true },
        new() { Id = 12, SpeciesId = 1, Name = "Cornish",                   Slug = "cornish",                   IsActive = true },
        new() { Id = 13, SpeciesId = 1, Name = "Dominant White",            Slug = "dominant-white",            IsActive = true },
        new() { Id = 14, SpeciesId = 1, Name = "Karmazyn",                  Slug = "karmazyn",                  IsActive = true },
        new() { Id = 15, SpeciesId = 1, Name = "Brojler",                   Slug = "brojler",                   IsActive = true },
        new() { Id = 41, SpeciesId = 1, Name = "Silka Jedwabista",          Slug = "silka-jedwabista",          IsActive = true },
        // Kaczki (2)
        new() { Id = 16, SpeciesId = 2, Name = "Pekin",                     Slug = "pekin",                     IsActive = true },
        new() { Id = 17, SpeciesId = 2, Name = "Krzyżówka",                 Slug = "krzyzowka",                 IsActive = true },
        new() { Id = 18, SpeciesId = 2, Name = "Biegus indyjski",           Slug = "biegus-indyjski",           IsActive = true },
        new() { Id = 19, SpeciesId = 2, Name = "Khaki Campbell",            Slug = "khaki-campbell",            IsActive = true },
        new() { Id = 20, SpeciesId = 2, Name = "Staropolska",               Slug = "staropolska",               IsActive = true },
        new() { Id = 21, SpeciesId = 2, Name = "Pomaska",                   Slug = "pomaska",                   IsActive = true },
        new() { Id = 22, SpeciesId = 2, Name = "Biała kołudzka",            Slug = "biala-koludzka",            IsActive = true },
        new() { Id = 23, SpeciesId = 2, Name = "Rouen",                     Slug = "rouen",                     IsActive = true },
        new() { Id = 24, SpeciesId = 2, Name = "Cayuga",                    Slug = "cayuga",                    IsActive = true },
        new() { Id = 25, SpeciesId = 2, Name = "Muskowa",                   Slug = "muskowa",                   IsActive = true },
        // Gęsi (3)
        new() { Id = 26, SpeciesId = 3, Name = "Biała kołudzka",            Slug = "biala-koludzka-ges",        IsActive = true },
        new() { Id = 27, SpeciesId = 3, Name = "Pomorska",                  Slug = "pomorska",                  IsActive = true },
        new() { Id = 28, SpeciesId = 3, Name = "Reńska",                    Slug = "renska",                    IsActive = true },
        new() { Id = 29, SpeciesId = 3, Name = "Tuluzka",                   Slug = "tuluzka",                   IsActive = true },
        new() { Id = 30, SpeciesId = 3, Name = "Landes",                    Slug = "landes",                    IsActive = true },
        new() { Id = 31, SpeciesId = 3, Name = "Kubańska",                  Slug = "kubanska",                  IsActive = true },
        new() { Id = 32, SpeciesId = 3, Name = "Bacuńska",                  Slug = "bacunska",                  IsActive = true },
        new() { Id = 33, SpeciesId = 3, Name = "Biała włoska",              Slug = "biala-wloska",              IsActive = true },
        new() { Id = 34, SpeciesId = 3, Name = "Gęsia domowa",              Slug = "gesia-domowa",              IsActive = true },
        new() { Id = 35, SpeciesId = 3, Name = "Emdenska",                  Slug = "emdenska",                  IsActive = true },
        // Indyki (4)
        new() { Id = 36, SpeciesId = 4, Name = "Brązowe",                   Slug = "brazowe",                   IsActive = true },
        new() { Id = 37, SpeciesId = 4, Name = "Białe holenderskie",        Slug = "biale-holenderskie",        IsActive = true },
        new() { Id = 38, SpeciesId = 4, Name = "Royal Palm",                Slug = "royal-palm",                IsActive = true },
        // Perliczki (5)
        new() { Id = 39, SpeciesId = 5, Name = "Perłowe",                   Slug = "perlowe",                   IsActive = true },
        new() { Id = 40, SpeciesId = 5, Name = "Białogardle",               Slug = "bialogardle",               IsActive = true },
        new() { Id = 42, SpeciesId = 5, Name = "Lawendowe",                 Slug = "lawendowe",                 IsActive = true },
        new() { Id = 43, SpeciesId = 5, Name = "Łaciate",                   Slug = "laciate",                   IsActive = true },
        // Przepiórki (6)
        new() { Id = 44, SpeciesId = 6, Name = "Japońska",                  Slug = "japonska",                  IsActive = true },
        new() { Id = 45, SpeciesId = 6, Name = "Królewska",                 Slug = "krolewska",                 IsActive = true },
        new() { Id = 46, SpeciesId = 6, Name = "Olbrzymia",                 Slug = "olbrzymia",                 IsActive = true },
        new() { Id = 47, SpeciesId = 6, Name = "Maniocka",                  Slug = "maniocka",                  IsActive = true },
        // Bażanty (7)
        new() { Id = 48, SpeciesId = 7, Name = "Złocisty",                  Slug = "zlocisty",                  IsActive = true },
        new() { Id = 49, SpeciesId = 7, Name = "Arystotelesa",              Slug = "arystotelesa",              IsActive = true },
        new() { Id = 50, SpeciesId = 7, Name = "Srebrny",                   Slug = "srebrny",                   IsActive = true },
        new() { Id = 51, SpeciesId = 7, Name = "Złoty",                     Slug = "zloty",                     IsActive = true },
        // Gołębie (8)
        new() { Id = 52, SpeciesId = 8, Name = "Pocztowe",                  Slug = "pocztowe",                  IsActive = true },
        new() { Id = 53, SpeciesId = 8, Name = "Tumbler",                   Slug = "tumbler",                   IsActive = true },
        new() { Id = 54, SpeciesId = 8, Name = "Pawiki",                    Slug = "pawiki",                    IsActive = true },
        new() { Id = 55, SpeciesId = 8, Name = "Królewskie",                Slug = "krolewskie-golebie",        IsActive = true },
        // Inne (9)
        new() { Id = 56, SpeciesId = 9, Name = "Afrykańskie",               Slug = "strusie-afrykanskie",       IsActive = true },
        new() { Id = 57, SpeciesId = 9, Name = "Emu",                       Slug = "emu",                       IsActive = true },
        new() { Id = 58, SpeciesId = 9, Name = "Nandu",                     Slug = "nandu",                     IsActive = true },
        new() { Id = 59, SpeciesId = 9, Name = "Pawiany",                   Slug = "pawiany",                   IsActive = true },
    ];
}