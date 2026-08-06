using AwesomeAssertions;
using Biegusowo.Tests.Common;
using BiegusowoApi.Domain.Conversations;
using BiegusowoApi.Domain.Dtos.Conversation;
using BiegusowoApi.Helpers;
using Microsoft.Extensions.Validation;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Biegusowo.Tests.ControllerTests;

public class ConversationsControllerTests(WebApplicationFactoryFixture factory) 
    : BaseTests(factory)
{
    [Fact]
    public async Task GetConversations_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        // Act
        var response = await client.GetAsync("/api/conversations",
            CancellationToken);
        // Assert
        response.Should().Be200Ok();

        var result = await response.Content.ReadFromJsonAsync<PaginatedList<MinimalConversationDto>>(CancellationToken);
        await Verify(result);
    }

    [Fact]
    public async Task GetConversations_SecondPage_DoesNotRepeatFirstPageItems()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var firstResponse = await client.GetAsync("/api/conversations?pageSize=1", CancellationToken);
        var firstPage = await firstResponse.Content.ReadFromJsonAsync<PaginatedList<MinimalConversationDto>>(CancellationToken);

        var last = firstPage!.Items.Last();

        var secondResponse = await client.GetAsync(
            $"/api/conversations?pageSize=5&beforeLastMessageAt={Uri.EscapeDataString(last.LastMessage.CreatedAt.ToString("O"))}&beforeConversationId={last.Id}",
            CancellationToken);

        secondResponse.Should().Be200Ok();

        var secondPage = await secondResponse.Content.ReadFromJsonAsync<PaginatedList<MinimalConversationDto>>(CancellationToken);

        await Verify(secondPage);
    }

    [Fact]
    public async Task GetMessagesReturnsMessages()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        string conversationId = "00000000-0000-0000-0000-000000000011";
        // Act
        var response = await client.GetAsync($"/api/conversations/{conversationId}",
            CancellationToken);
        // Assert
        response.Should().Be200Ok();
        var result = await response.Content.ReadFromJsonAsync<ConversationDto>(CancellationToken);
        await Verify(result);
    }

    //[Fact]
    //public async Task GetMessagesWithPaginationReturnsPagedMessages()
    //{
    //    // Arrange
    //    var client = _factory.CreateAuthenticatedClient(FirstUserId);
    //    string conversationId = "00000000-0000-0000-0000-000000000019";
    //    DateTimeOffset date = new DateTimeOffset(2026, 7, 19, 19, 15, 47, 884, TimeSpan.Zero);
    //    // Act
    //    var response = await client.GetAsync(
    //        $"/api/conversations/{conversationId}?pageSize=3" +
    //        $"&beforeCreatedAt={Uri.EscapeDataString(date.ToString("O"))}" +
    //        $"&beforeMessageId=00000000-0000-0000-0000-000000000235",
    //        CancellationToken);
    //    // Assert
    //    response.Should().Be200Ok();
    //    var result = await response.Content.ReadFromJsonAsync<ConversationDto>(CancellationToken);
    //    await Verify(result);
    //}

    [Fact]
    public async Task GetMessages_NotParticipant_Returns404NotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        string conversationId = "c";
        // Act
        var response = await client.GetAsync($"/api/conversations/{conversationId}",
            CancellationToken);
        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task CreateConversationCreatesConversation()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000000009");

        var request = new ConversationRequest(
            ListingId: Guid.Parse("00000000-0000-0000-0000-000000000015"),
            FirstMessage: "Hello, I am interested in your listing.",
            SentAt: DateTimeOffset.UtcNow
        );

        // Act
        var response = await PostAsJsonAsync(
            client,
            "/api/conversations",
            request,
            CancellationToken);

        // Assert
        response.Should().Be200Ok();
        ConversationDto? conversation = await response.Content.ReadFromJsonAsync<ConversationDto>(CancellationToken);
        await Verify(conversation);
    }

    [Fact]
    public async Task CreateConversation_Existing_ReturnsConflict()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var request = new ConversationRequest(
            ListingId: Guid.Parse("00000000-0000-0000-0000-000000000011"),
            FirstMessage: "This is already existing conversation.",
            SentAt: DateTimeOffset.UtcNow
        );

        // Act
        var response = await PostAsJsonAsync(
            client,
            "/api/conversations",
            request,
            CancellationToken);

        // Assert
        response.Should().Be409Conflict();
    }

    [Fact]
    public async Task CreateConversation_OwnedListing_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var request = new ConversationRequest(
            ListingId: Guid.Parse("00000000-0000-0000-0000-000000000012"),
            FirstMessage: "This is already existing conversation.",
            SentAt: DateTimeOffset.UtcNow
        );

        // Act
        var response = await PostAsJsonAsync(
            client,
            "/api/conversations",
            request,
            CancellationToken);

        // Assert
        response.Should().Be400BadRequest();
    }
}

