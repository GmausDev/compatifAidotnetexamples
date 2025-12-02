using CompactifAI.Client;
using CompactifAI.Client.Models;
using FluentAssertions;
using System.Net;

namespace CompactifAI.Example.Tests;

public class CompactifAIClientTests
{
    #region Model Constants Tests

    [Fact]
    public void CompactifAIModels_ShouldHaveCorrectModelIdentifiers()
    {
        // Arrange & Act & Assert
        CompactifAIModels.DeepSeekR1_Slim.Should().NotBeNullOrEmpty();
        CompactifAIModels.Llama4Scout_Slim.Should().NotBeNullOrEmpty();
        CompactifAIModels.Llama4Scout.Should().NotBeNullOrEmpty();
        CompactifAIModels.Llama33_70B_Slim.Should().NotBeNullOrEmpty();
        CompactifAIModels.Llama33_70B.Should().NotBeNullOrEmpty();
        CompactifAIModels.Llama31_8B_Slim.Should().NotBeNullOrEmpty();
        CompactifAIModels.Llama31_8B.Should().NotBeNullOrEmpty();
        CompactifAIModels.MistralSmall31_Slim.Should().NotBeNullOrEmpty();
        CompactifAIModels.MistralSmall31.Should().NotBeNullOrEmpty();
        CompactifAIModels.WhisperLargeV3.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CompactifAIModels_SlimVersions_ShouldContainSlimSuffix()
    {
        // Assert - Slim models should be identifiable
        CompactifAIModels.DeepSeekR1_Slim.Should().Contain("slim");
        CompactifAIModels.Llama4Scout_Slim.Should().Contain("slim");
        CompactifAIModels.Llama33_70B_Slim.Should().Contain("slim");
        CompactifAIModels.Llama31_8B_Slim.Should().Contain("slim");
        CompactifAIModels.MistralSmall31_Slim.Should().Contain("slim");
    }

    #endregion

    #region ChatMessage Factory Tests

    [Fact]
    public void ChatMessage_System_ShouldCreateSystemMessage()
    {
        // Arrange
        var content = "You are a helpful assistant.";

        // Act
        var message = ChatMessage.System(content);

        // Assert
        message.Role.Should().Be("system");
        message.Content.Should().Be(content);
    }

    [Fact]
    public void ChatMessage_User_ShouldCreateUserMessage()
    {
        // Arrange
        var content = "Hello, how are you?";

        // Act
        var message = ChatMessage.User(content);

        // Assert
        message.Role.Should().Be("user");
        message.Content.Should().Be(content);
    }

    [Fact]
    public void ChatMessage_Assistant_ShouldCreateAssistantMessage()
    {
        // Arrange
        var content = "I'm doing well, thank you!";

        // Act
        var message = ChatMessage.Assistant(content);

        // Assert
        message.Role.Should().Be("assistant");
        message.Content.Should().Be(content);
    }

    #endregion

    #region ChatCompletionRequest Tests

    [Fact]
    public void ChatCompletionRequest_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var request = new ChatCompletionRequest();

        // Assert - verify the request object is created
        request.Should().NotBeNull();
    }

    [Fact]
    public void ChatCompletionRequest_ShouldAcceptAllProperties()
    {
        // Arrange & Act
        var request = new ChatCompletionRequest
        {
            Model = CompactifAIModels.Llama31_8B_Slim,
            Messages = new List<ChatMessage>
            {
                ChatMessage.System("Be concise."),
                ChatMessage.User("Hello!")
            },
            Temperature = 0.7f,
            MaxTokens = 100,
            Stream = false
        };

        // Assert
        request.Model.Should().Be(CompactifAIModels.Llama31_8B_Slim);
        request.Messages.Should().HaveCount(2);
        request.Temperature.Should().Be(0.7f);
        request.MaxTokens.Should().Be(100);
        request.Stream.Should().BeFalse();
    }

    #endregion

    #region CompletionRequest Tests

    [Fact]
    public void CompletionRequest_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var request = new CompletionRequest();

        // Assert - verify the request object is created
        request.Should().NotBeNull();
    }

    [Fact]
    public void CompletionRequest_ShouldAcceptAllProperties()
    {
        // Arrange & Act
        var request = new CompletionRequest
        {
            Model = CompactifAIModels.Llama31_8B_Slim,
            Prompt = "Complete this sentence:",
            Temperature = 0.5f,
            MaxTokens = 50
        };

        // Assert
        request.Model.Should().Be(CompactifAIModels.Llama31_8B_Slim);
        request.Prompt.Should().Be("Complete this sentence:");
        request.Temperature.Should().Be(0.5f);
        request.MaxTokens.Should().Be(50);
    }

    #endregion

    #region TranscriptionRequest Tests

    [Fact]
    public void TranscriptionRequest_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var request = new TranscriptionRequest();

        // Assert - verify the request object is created
        request.Should().NotBeNull();
    }

    [Fact]
    public void TranscriptionRequest_ShouldAcceptFileContent()
    {
        // Arrange
        var fileContent = new byte[] { 0x00, 0x01, 0x02 };

        // Act
        var request = new TranscriptionRequest
        {
            Model = CompactifAIModels.WhisperLargeV3,
            FileContent = fileContent,
            FileName = "audio.mp3",
            Language = "en"
        };

        // Assert
        request.Model.Should().Be(CompactifAIModels.WhisperLargeV3);
        request.FileContent.Should().BeEquivalentTo(fileContent);
        request.FileName.Should().Be("audio.mp3");
        request.Language.Should().Be("en");
    }

    #endregion

    #region CompactifAIOptions Tests

    [Fact]
    public void CompactifAIOptions_ShouldHaveCorrectSectionName()
    {
        // Assert
        CompactifAIOptions.SectionName.Should().Be("CompactifAI");
    }

    [Fact]
    public void CompactifAIOptions_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var options = new CompactifAIOptions();

        // Assert
        options.Should().NotBeNull();
        options.BaseUrl.Should().NotBeNullOrEmpty();
        options.TimeoutSeconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CompactifAIOptions_ShouldAcceptCustomValues()
    {
        // Arrange & Act
        var options = new CompactifAIOptions
        {
            ApiKey = "test-api-key",
            BaseUrl = "https://custom.api.com/v1",
            DefaultModel = CompactifAIModels.Llama31_8B_Slim,
            TimeoutSeconds = 60
        };

        // Assert
        options.ApiKey.Should().Be("test-api-key");
        options.BaseUrl.Should().Be("https://custom.api.com/v1");
        options.DefaultModel.Should().Be(CompactifAIModels.Llama31_8B_Slim);
        options.TimeoutSeconds.Should().Be(60);
    }

    #endregion

    #region CompactifAIException Tests

    [Fact]
    public void CompactifAIException_ShouldStoreMessage()
    {
        // Arrange & Act
        var exception = new CompactifAIException("Test error message");

        // Assert
        exception.Message.Should().Be("Test error message");
    }

    [Fact]
    public void CompactifAIException_ShouldStoreStatusCodeAndResponseBody()
    {
        // Arrange & Act
        var exception = new CompactifAIException(
            "API Error",
            HttpStatusCode.BadRequest,
            "{\"error\": \"Invalid request\"}");

        // Assert
        exception.Message.Should().Be("API Error");
        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ResponseBody.Should().Be("{\"error\": \"Invalid request\"}");
    }

    [Fact]
    public void CompactifAIException_ShouldStoreInnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new CompactifAIException("Outer error", innerException);

        // Assert
        exception.Message.Should().Be("Outer error");
        exception.InnerException.Should().Be(innerException);
    }

    #endregion

    #region Client Constructor Tests

    [Fact]
    public void CompactifAIClient_ShouldCreateWithApiKey()
    {
        // Arrange & Act
        var client = new CompactifAIClient("test-api-key");

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void CompactifAIClient_ShouldCreateWithApiKeyAndBaseUrl()
    {
        // Arrange & Act
        var client = new CompactifAIClient("test-api-key", "https://custom.api.com/v1");

        // Assert
        client.Should().NotBeNull();
    }

    #endregion
}
