using AwesomeAssertions;
using Biegusowo.Tests.Common;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Features.Listings.Dtos;
using BiegusowoApi.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Biegusowo.Tests.ControllerTests;

public class ListingsControllerTests(WebApplicationFactoryFixture factory) 
    : BaseTests(factory)
{
    private static VerifySettings CreateSettings()
    {
        var settings = new VerifySettings();
        settings.DontScrubDateTimes();
        return settings;
    }

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
            .ReadFromJsonAsync<PaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        result.Items.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetListings_FilterBySpecies_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?speciesId=1&pageSize=5",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_FilterByBreed_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?breedId=5&pageSize=5",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_FilterByCity_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?city=Mackhaven&pageSize=5",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_Search_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?search=Listing+number+1&pageSize=5",
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
            "/api/listings?priceMin=10&priceMax=30&pageSize=5",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    [Fact]
    public async Task GetListings_Paging_ReturnsRequestedPage()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        // Act - get first page
        var firstResponse = await client.GetAsync(
            "/api/listings?pageSize=5",
            CancellationToken);

        firstResponse.Should().Be200Ok();

        var firstPage = await firstResponse.Content
            .ReadFromJsonAsync<CursorPaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        firstPage!.Items.Should().HaveCount(5);
        firstPage.HasNextPage.Should().BeTrue();

        var last = firstPage.Items.Last();

        // Act - get second page using cursor from the last item of page 1
        var secondResponse = await client.GetAsync(
            $"/api/listings?pageSize=5&beforeCursorValue={Uri.EscapeDataString(last.CreatedAt.ToString("O"))}&beforeListingId={last.Id}",
            CancellationToken);

        secondResponse.Should().Be200Ok();

        var secondPage = await secondResponse.Content
            .ReadFromJsonAsync<CursorPaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        // Assert
        await Verify(secondPage);
    }

    [Theory]
    [InlineData("price_asc")]
    [InlineData("price_desc")]
    [InlineData("newest")]
    [InlineData("oldest")]
    public async Task GetListings_Sorting_ReturnsSortedResults(string sort)
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            $"/api/listings?sort={sort}&pageSize=5",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        await Verify(result)
            .DontScrubDateTimes()
            .UseParameters(sort);
    }

    [Fact]
    public async Task GetListings_FilterBySpeciesAndBreed_ReturnsMatchingListings()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var response = await client.GetAsync(
            "/api/listings?speciesId=7&breedId=48&pageSize=5",
            CancellationToken);

        response.Should().Be200Ok();

        var result = await response.Content
            .ReadFromJsonAsync<PaginatedList<MinimalListingDto>>(cancellationToken: CancellationToken);

        await Verify(result);
    }

    // ─── GetListing ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task GetListing_ReturnsListing()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        const string listingId = "00000000-0000-0000-0000-000000000001";

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
            new Dictionary<string, string>() { ["0"] = "https://example.com/image1.jpg" }
        );

        // Act
        var response = await PostAsJsonAsync(
            client,
            "/api/listings",
            request,
            CancellationToken);

        // Assert
        response.Should().Be201Created();

        var listing = await response.Content.ReadFromJsonAsync<ListingDto>(
            cancellationToken: CancellationToken);

        Listing? entity = await GetQueryable<Listing>()
            .FirstOrDefaultAsync(x => x.Id == listing!.Id, CancellationToken);

        entity.Should().NotBeNull();
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
            client,
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
            client,
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
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000000063");
        string listingId = "00000000-0000-0000-0000-000000000100";
        // Act
        var response = await client.DeleteAsync($"/api/listings/{listingId}",
            CancellationToken);

        response.Should().Be204NoContent();
    }

    [Fact]
    public async Task DeleteNonExistentListing_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        string listingId = "non-existend-id";
        // Act
        var response = await client.DeleteAsync($"/api/listings/{listingId}",
            CancellationToken);

        response.Should().Be404NotFound();
    }

    [Fact] 
    public async Task DeleteNotOwnedListing_ReturnsForbid()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        string listingId = "00000000-0000-0000-0000-000000000094";
        // Act
        var response = await client.DeleteAsync($"/api/listings/{listingId}",
            CancellationToken);

        response.Should().Be403Forbidden();
    }

}