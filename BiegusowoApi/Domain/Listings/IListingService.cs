using Ardalis.Result;
using BiegusowoApi.Domain.Dtos.Listing;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace BiegusowoApi.Domain.Listings;

public interface IListingService
{
    Task<CursorPaginatedList<MinimalListingDto>> GetListingsAsync(
        string? search,
        string? city,
        string? sort,
        int? speciesId,
        int? breedId,
        int? priceMin,
        int? priceMax,
        string? beforeCursorValue,
        Guid? beforeListingId,
        int pageSize);

    Task<Result<ListingDto>> GetListingAsync(Guid id);

    Task<Result<ListingDto>> CreateListingAsync(Guid userId, CreateListingRequest request);

    Task<Result<ListingDto>> PatchListingAsync(
       Guid userId,
       Guid id,
       JsonPatchDocument<UpdateListingRequest> patch);

    Task<Result> DeleteListingAsync(Guid userId, Guid id);
}
