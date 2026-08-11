using Ardalis.Result;
using BiegusowoApi.Features.Listings.Dtos;
using BiegusowoApi.Shared.Helpers;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace BiegusowoApi.Features.Listings;

public interface IListingService
{
    Task<Result<CursorPaginatedList<MinimalListingDto>>> GetListingsAsync(GetListingsRequest request);

    Task<Result<ListingDto>> GetListingAsync(Guid id);

    Task<Result<ListingDto>> CreateListingAsync(Guid userId, CreateListingRequest request);

    Task<Result<ListingDto>> PatchListingAsync(
       Guid userId,
       Guid id,
       JsonPatchDocument<UpdateListingRequest> patch);

    Task<Result> DeleteListingAsync(Guid userId, Guid id);
}
