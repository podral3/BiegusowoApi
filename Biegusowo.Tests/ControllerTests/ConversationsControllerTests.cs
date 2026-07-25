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
    public async Task GetMessagesReturnsMessages()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        string conversationId = "00000000-0000-0000-0000-000000000010";
        // Act
        var response = await client.GetAsync($"/api/conversations/{conversationId}",
            CancellationToken);
        // Assert
        response.Should().Be200Ok();
        var result = await response.Content.ReadFromJsonAsync<ConversationDto>(CancellationToken);
        await Verify(result);
    }

    [Fact]
    public async Task GetMessagesWithPaginationReturnsPagedMessages()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        string conversationId = "00000000-0000-0000-0000-000000000010";
        // Act
        var response = await client.GetAsync($"/api/conversations/{conversationId}?skip=5&pageSize=3",
            CancellationToken);
        // Assert
        response.Should().Be200Ok();
        var result = await response.Content.ReadFromJsonAsync<ConversationDto>(CancellationToken);
        await Verify(result);
    }

    [Fact]
    public async Task GetMessages_NotParticipant_Returns403Forbid()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        string conversationId = "019f71d5-31a5-7528-bd14-de51056f545c";
        // Act
        var response = await client.GetAsync($"/api/conversations/{conversationId}?skip=5&pageSize=10",
            CancellationToken);
        // Assert
        response.Should().Be403Forbidden();
    }

    [Fact]
    public async Task CreateConversationCreatesConversation()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);

        var request = new ConversationRequest(
            Guid.Parse("019f71d5-3185-7730-892e-0ebe47e7e69e"),
            "Hello, I am interested in your listing.",
            DateTimeOffset.UtcNow
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
            Guid.Parse("019f71d5-31a6-75e0-9fb2-5919ec6462ba"),
            "Hello, I am interested in your listing.",
            DateTimeOffset.UtcNow
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
}

