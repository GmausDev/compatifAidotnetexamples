using CompactifAI.Client;
using CompactifAI.Client.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CompactifAI.Example.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddCompactifAI_WithApiKey_ShouldRegisterClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCompactifAI("test-api-key");
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<ICompactifAIClient>();

        // Assert
        client.Should().NotBeNull();
        client.Should().BeOfType<CompactifAIClient>();
    }

    [Fact]
    public void AddCompactifAI_WithOptions_ShouldRegisterClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCompactifAI(options =>
        {
            options.ApiKey = "test-api-key";
            options.BaseUrl = "https://custom.api.com/v1";
            options.DefaultModel = CompactifAIModels.Llama31_8B_Slim;
            options.TimeoutSeconds = 120;
        });
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<ICompactifAIClient>();

        // Assert
        client.Should().NotBeNull();
        client.Should().BeOfType<CompactifAIClient>();
    }

    [Fact]
    public void AddCompactifAI_ShouldResolveMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCompactifAI("test-api-key");
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var client1 = serviceProvider.GetService<ICompactifAIClient>();
        var client2 = serviceProvider.GetService<ICompactifAIClient>();

        // Assert - both should be valid instances
        client1.Should().NotBeNull();
        client2.Should().NotBeNull();
    }

    [Fact]
    public void ICompactifAIClient_ShouldBeResolvable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCompactifAI(options =>
        {
            options.ApiKey = "test-api-key";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var client = serviceProvider.GetRequiredService<ICompactifAIClient>();

        // Assert
        client.Should().NotBeNull();
    }
}
