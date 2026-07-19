using AwesomeAssertions;
using Biegusowo.Tests.Common;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.Dtos.Listing;
using BiegusowoApi.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Biegusowo.Tests.ControllerTests;

public class ListingsControllerTests(WebApplicationFactoryFixture factory) : BaseTests(factory)
{
    // ─── GetListings ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetListings_ReturnsFirstPage()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        // Act
        var response = await client.GetAsync("/api/listings", CancellationToken);

        // Assert
        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_FilterBySpecies_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?speciesId=1",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_FilterByBreed_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?breedId=10",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_FilterByCity_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?city=Rosenbaumton",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_Search_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?search=Rosenbaumton",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_FilterByPriceRange_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?priceMin=10&priceMax=30",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_Paging_ReturnsRequestedPage()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?page=2&pageSize=5",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Theory]
    [InlineData("price")]
    [InlineData("price_desc")]
    [InlineData("created")]
    [InlineData("created_desc")]
    public async Task GetListings_Sorting_ReturnsSortedResults(string sort)
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            $"/api/listings?sort={sort}",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result)
            .UseParameters(sort);
    }

    [Fact]
    public async Task GetListings_FilterBySpeciesAndBreed_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?speciesId=1&breedId=10",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<ListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    // ─── GetListing ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task GetListing_ReturnsListing()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        const string listingId = "019f71d5-31a7-7fb7-8508-381e3881c3d9";

        // Act
        var response = await client.GetAsync(
            $"/api/listings/{listingId}",
            CancellationToken);

        // Assert
        response.Should().Be200Ok();

        var result = await response.Content.ReadFromJsonAsync<ListingDto>(
            cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListing_WhenListingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        const string listingId = "019f71d5-0000-7000-8000-000000000000";

        // Act
        var response = await client.GetAsync(
            $"/api/listings/{listingId}",
            CancellationToken);

        // Assert
        response.Should().Be404NotFound();
    }

    // ─── PostListing ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task CreateListing_ReturnsCreatedListing()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var request = new CreateListingRequest(
            Title: "Golden Retriever Puppies",
            Description: "Healthy puppies ready for adoption.",
            Price: 1500,
            PriceNegotiable: true,
            SpeciesId: 1,
            BreedId: 10,
            BreedNote: "",
            VoivodeshipId: 8,
            ListingTypeId: 1,
            ListingStatusId: 1,
            CityName: "Warsaw",
            Longitude: 21.0122,
            Latitude: 52.2297,
            ImageIds: []
        );

        // Act
        var response = await PostAsJsonAsync(
            "/api/listings",
            request,
            CancellationToken);

        // Assert
        response.Should().Be201Created();

        var listing = await response.Content.ReadFromJsonAsync<ListingDto>(
            cancellationToken: CancellationToken);

        await Verify(listing);
    }

    [Fact]
    public async Task CreateListing_PersistsListing()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var request = new CreateListingRequest(
            "Integration Test Listing",
            "Description",
            99,
            false,
            1,
            10,
            "",
            8,
            1,
            1,
            "Warsaw",
            21.0,
            52.0,
            []
        );

        // Act
        var response = await PostAsJsonAsync(
            "/api/listings",
            request,
            CancellationToken);

        response.Should().Be201Created();

        var listing = await response.Content.ReadFromJsonAsync<ListingDto>(
            cancellationToken: CancellationToken);

        // Assert
        var entity = await GetQueryable<Listing>()
            .FirstOrDefaultAsync(x => x.Id.ToString() == listing!.Id, CancellationToken);

        entity.Title.Should().Be(request.Title);
        entity.Description.Should().Be(request.Description);
        entity.Price.Should().Be(request.Price);
        entity.SpeciesId.Should().Be(request.SpeciesId);
        entity.BreedId.Should().Be(request.BreedId);
        entity.CityName.Should().Be(request.CityName);
    }

    [Fact]
    public async Task CreateListing_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new CreateListingRequest(
            "Test",
            "Test",
            100,
            false,
            1,
            10,
            "",
            8,
            1,
            1,
            "Warsaw",
            21,
            52,
            []
        );

        // Act
        var response = await PostAsJsonAsync(
            "/api/listings",
            request,
            CancellationToken);

        // Assert
        response.Should().Be401Unauthorized();
    }

    [Fact]
    public async Task CreateListing_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var request = new CreateListingRequest(
            "",
            "",
            -100,
            false,
            0,
            0,
            "",
            0,
            0,
            0,
            "",
            0,
            0,
            []
        );

        // Act
        var response = await PostAsJsonAsync(
            "/api/listings",
            request,
            CancellationToken);

        // Assert
        response.Should().Be400BadRequest();
    }
    // ─── DeleteListing ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task DeleteListing_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(userId: "019f71d5-2ed0-710a-afe7-6b90b1aa8505");
        string listingId = "019f71d5-31a7-7fb7-8508-381e3881c3d9";
        // Act
        var response = await client.DeleteAsync($"/api/listings/{listingId}",
            CancellationToken);

        response.Should().Be200Ok();
    }

    [Fact]
    public async Task DeleteNonExistentListing_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(userId: "019f71d5-2ed0-710a-afe7-6b90b1aa8505");
        string listingId = "non-existend-id";
        // Act
        var response = await client.DeleteAsync($"/api/listings/{listingId}",
            CancellationToken);

        response.Should().Be404NotFound();
    }

}