using Biegusowo.Tests.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biegusowo.Tests.ControllerTests;

public class ConversationsControllerTests(WebApplicationFactoryFixture factory) 
    : BaseTests(factory)
{
    [Fact]
    public async Task GetConversations_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var response = await client.GetAsync("/api/conversations");
        // Assert
        response.EnsureSuccessStatusCode();
    }
}

