using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Types;
using BiegusowoApi.Domain.Dtos.Listing;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text;
using System.Text.Json;

namespace BiegusowoApi.Domain.Listings;


public class ListingsService(ApplicationDbContext db) : IListingService
{

    private readonly ApplicationDbContext _dbContext = db;
    GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory();
    private const string SortNewest = "newest";
    private const string SortOldest = "oldest";
    private const string SortPriceAsc = "price_asc";
    private const string SortPriceDesc = "price_desc";

    public async Task<CursorPaginatedList<MinimalListingDto>> GetListingsAsync(
        string? search,
        string? city,
        string? sort,
        int? speciesId,
        int? breedId,
        int? priceMin,
        int? priceMax,
        string? beforeCursorValue,
        Guid? beforeListingId,
        int pageSize)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        sort = string.IsNullOrWhiteSpace(sort) ? SortNewest : sort.ToLowerInvariant();

        IQueryable<Listing> query = _dbContext.Listings
            .AsNoTracking()
            .Where(l => l.DeletedAt == null && l.ListingStatus == ListingStatus.Active);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(l => EF.Functions.ILike(l.Title, $"%{term}%")
                                   || EF.Functions.ILike(l.Description, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(l => EF.Functions.ILike(l.CityName, city));

        if (speciesId.HasValue)
            query = query.Where(l => l.SpeciesId == speciesId.Value);

        if (breedId.HasValue)
            query = query.Where(l => l.BreedId == breedId.Value);

        if (priceMin.HasValue)
            query = query.Where(l => l.Price >= priceMin.Value);

        if (priceMax.HasValue)
            query = query.Where(l => l.Price <= priceMax.Value);

        // Ordering
        query = sort switch
        {
            SortOldest => query.OrderBy(l => l.CreatedAt).ThenBy(l => l.Id),
            SortPriceAsc => query.OrderBy(l => l.Price).ThenBy(l => l.Id),
            SortPriceDesc => query.OrderByDescending(l => l.Price).ThenByDescending(l => l.Id),
            _ /* newest */ => query.OrderByDescending(l => l.CreatedAt).ThenByDescending(l => l.Id),
        };

        // Cursor (keyset pagination)
        if (beforeCursorValue is not null && beforeListingId.HasValue)
        {
            query = sort switch
            {
                SortOldest => ApplyDateCursor(query, beforeCursorValue, beforeListingId.Value, ascending: true),
                SortPriceAsc => ApplyPriceCursor(query, beforeCursorValue, beforeListingId.Value, ascending: true),
                SortPriceDesc => ApplyPriceCursor(query, beforeCursorValue, beforeListingId.Value, ascending: false),
                _ => ApplyDateCursor(query, beforeCursorValue, beforeListingId.Value, ascending: false),
            };
        }

        var items = await query
            .Take(pageSize + 1)
            .ToListAsync();

        var hasNextPage = items.Count > pageSize;
        if (hasNextPage)
            items = items.Take(pageSize).ToList();

        var dtos = items.Select(MapToMinimalDto).ToList();

        return new CursorPaginatedList<MinimalListingDto>(dtos, hasNextPage);
    }

    private static IQueryable<Listing> ApplyDateCursor(
        IQueryable<Listing> query, string cursorValue, Guid cursorId, bool ascending)
    {
        if (!long.TryParse(cursorValue, out var ticks))
            return query; // invalid cursor -> ignore rather than error

        var cursorDate = new DateTimeOffset(ticks, TimeSpan.Zero);

        return ascending
            ? query.Where(l => l.CreatedAt > cursorDate
                             || (l.CreatedAt == cursorDate && l.Id > cursorId))
            : query.Where(l => l.CreatedAt < cursorDate
                             || (l.CreatedAt == cursorDate && l.Id < cursorId));
    }

    private static IQueryable<Listing> ApplyPriceCursor(
        IQueryable<Listing> query, string cursorValue, Guid cursorId, bool ascending)
    {
        if (!double.TryParse(cursorValue, out var cursorPrice))
            return query;

        return ascending
            ? query.Where(l => l.Price > cursorPrice
                             || (l.Price == cursorPrice && l.Id > cursorId))
            : query.Where(l => l.Price < cursorPrice
                             || (l.Price == cursorPrice && l.Id < cursorId));
    }

    public async Task<Result<ListingDto>> GetListingAsync(Guid id)
    {
        var listing = await _dbContext.Listings
            .AsNoTracking()
            .Include(l => l.User)
            .Include(l => l.Voivodeship)
            .FirstOrDefaultAsync(l => l.Id == id && l.DeletedAt == null);

        if (listing is null)
            return Result<ListingDto>.Failure(ServiceError.NotFound);

        return Result<ListingDto>.Success(MapToDto(listing));
    }

    public async Task<Result<ListingDto>> CreateListingAsync(Guid userId, CreateListingRequest request)
    {
        CreateListingRequestValidator validator = new();
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            return Result<ListingDto>.Failure(ServiceError.ValidationError);
        }

        var location = _geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));

        var orderedImageIds = request.ImageOrderIdPair
        .OrderBy(kvp => int.Parse(kvp.Key))
        .Select(kvp => kvp.Value)
        .ToArray();
        var imagesJson = JsonSerializer.Serialize(orderedImageIds);

        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            ListingType = (ListingType)request.ListingTypeId,
            ListingStatus = ListingStatus.Active,
            SpeciesId = request.SpeciesId,
            BreedId = request.BreedId,
            BreedNote = request.BreedNote,
            Price = request.Price,
            PriceNegotiable = request.PriceNegotiable,
            VoivodeshipId = request.VoivodeshipId,
            CityName = request.CityName,
            Location = location,
            Images = JsonDocument.Parse(imagesJson),
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _dbContext.Listings.Add(listing);
        await _dbContext.SaveChangesAsync();

        // Reload with navigations for the response DTO.
        await _dbContext.Entry(listing).Reference(l => l.User).LoadAsync();
        await _dbContext.Entry(listing).Reference(l => l.Species).LoadAsync();
        await _dbContext.Entry(listing).Reference(l => l.Breed).LoadAsync();
        await _dbContext.Entry(listing).Reference(l => l.Voivodeship).LoadAsync();

        return Result<ListingDto>.Success(MapToDto(listing), StatusCodes.Status201Created);
    }

    public async Task<Result<ListingDto>> PatchListingAsync(
        Guid userId, Guid id, JsonPatchDocument<UpdateListingRequest> patch)
    {
        var listing = await _dbContext.Listings
            .Include(l => l.User)
            .Include(l => l.Species)
            .Include(l => l.Breed)
            .Include(l => l.Voivodeship)
            .FirstOrDefaultAsync(l => l.Id == id && l.DeletedAt == null);

        if (listing is null)
            return Result<ListingDto>.Failure(ServiceError.NotFound);

        if (listing.UserId != userId)
            return Result<ListingDto>.Failure(ServiceError.Forbidden);

        var updateModel = new UpdateListingRequest
        {
            Title = listing.Title,
            Description = listing.Description,
            ListingType = listing.ListingType,
            ListingStatus = listing.ListingStatus,
            SpeciesId = listing.SpeciesId,
            BreedId = listing.BreedId,
            BreedNote = listing.BreedNote,
            Price = listing.Price,
            PriceNegotiable = listing.PriceNegotiable,
            VoivodeshipId = listing.VoivodeshipId,
            CityName = listing.CityName,
            Latitude = listing.Location.Y,
            Longitude = listing.Location.X,
        };

        patch.ApplyTo(updateModel);

        listing.Title = updateModel.Title;
        listing.Description = updateModel.Description;
        listing.ListingType = updateModel.ListingType;
        listing.ListingStatus = updateModel.ListingStatus;
        listing.SpeciesId = updateModel.SpeciesId;
        listing.BreedId = updateModel.BreedId;
        listing.BreedNote = updateModel.BreedNote;
        listing.Price = updateModel.Price;
        listing.PriceNegotiable = updateModel.PriceNegotiable;
        listing.VoivodeshipId = updateModel.VoivodeshipId;
        listing.CityName = updateModel.CityName;
        listing.Location = _geometryFactory.CreatePoint(new Coordinate(updateModel.Longitude, updateModel.Latitude));
        listing.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Result<ListingDto>.Success(MapToDto(listing));
    }

    public async Task<Result> DeleteListingAsync(Guid userId, Guid id)
    { 
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id && l.DeletedAt == null);

        if (listing is null)
            return Result.Failure(ServiceError.NotFound);

        if (listing.UserId != userId)
            return Result.Failure(ServiceError.Forbidden);

        listing.DeletedAt = DateTimeOffset.UtcNow;
        listing.ListingStatus = ListingStatus.Removed;

        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    private static ListingDto MapToDto(Listing l) => new(
        l.Id,
        l.UserId,
        l.User.DisplayName,
        l.Title,
        l.Description,
        l.ListingType,
        l.ListingStatus,
        l.BreedNote ?? string.Empty,
        l.Price,
        l.PriceNegotiable,
        l.SpeciesId,
        l.BreedId,
        ParseImages(l.Images),
        l.CreatedAt,
        l.UpdatedAt,
        l.VoivodeshipId,
        l.CityName);

    private static MinimalListingDto MapToMinimalDto(Listing l) => new(
        l.Id.ToString(),
        l.Title,
        l.Price,
        l.CityName,
        l.SpeciesId,
        l.BreedId,
        (int)l.ListingType,
        GetFirstImageUrl(l.Images), // ImageUrl comes later
        Slugify(l.Title, l.Id),
        l.CreatedAt,
        l.Location.Y,
        l.Location.X);

    private static string Slugify(string title, Guid id)
    {
        var sb = new StringBuilder();
        foreach (var c in title.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(c)) sb.Append(c);
            else if (sb.Length > 0 && sb[^1] != '-') sb.Append('-');
        }
        var slug = sb.ToString().Trim('-');
        return $"{slug}-{id.ToString()[..8]}";
    }
    private static Dictionary<string, string> ParseImages(JsonDocument images)
    {
        var root = images.RootElement;
        if (root.ValueKind != JsonValueKind.Array)
            return new Dictionary<string, string>();

        return root
            .EnumerateArray()
            .Select((element, index) => (index, value: element.GetString()))
            .Where(x => x.value is not null)
            .ToDictionary(x => x.index.ToString(), x => x.value!);
    }
    private static string GetFirstImageUrl(JsonDocument images)
    {
        var root = images.RootElement;
        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
            return string.Empty;

        var firstImageId = root[0].GetString();
        return firstImageId is null
            ? string.Empty
            : firstImageId;
    }
}