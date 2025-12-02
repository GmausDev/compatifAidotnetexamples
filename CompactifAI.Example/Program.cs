using CompactifAI.Client;
using CompactifAI.Client.Models;
using Microsoft.Extensions.DependencyInjection;

// =============================================================================
// CompactifAI.Client Example - .NET Console Application
// =============================================================================
// This example demonstrates how to use the CompactifAI.Client library
// for AI chat completions, text completions, and audio transcription.
// =============================================================================

Console.WriteLine("=== CompactifAI.Client Example ===\n");

// -----------------------------------------------------------------------------
// Option 1: Direct instantiation (simple usage)
// -----------------------------------------------------------------------------
var apiKey = Environment.GetEnvironmentVariable("COMPACTIFAI_API_KEY") ?? "you-key";
var baseUrl = Environment.GetEnvironmentVariable("COMPACTIFAI_BASE_URL"); // Optional
Console.WriteLine(apiKey);
var client = new CompactifAIClient(apiKey, baseUrl);

// -----------------------------------------------------------------------------
// Option 2: Using Dependency Injection (recommended for ASP.NET Core apps)
// -----------------------------------------------------------------------------
// var services = new ServiceCollection();
// services.AddCompactifAI(options =>
// {
//     options.ApiKey = apiKey;
//     options.BaseUrl = baseUrl ?? "https://api.compactif.ai/v1";
// });
// var serviceProvider = services.BuildServiceProvider();
// var client = serviceProvider.GetRequiredService<ICompactifAIClient>();

try
{
    // -------------------------------------------------------------------------
    // Example 1: List available models
    // -------------------------------------------------------------------------
    Console.WriteLine("1. Listing available models...");
    var modelsResponse = await client.ListModelsAsync();
    Console.WriteLine($"   Found {modelsResponse.Data.Count} models:");
    foreach (var model in modelsResponse.Data.Take(5))
    {
        Console.WriteLine($"   - {model.Id}");
    }
    Console.WriteLine();

    // -------------------------------------------------------------------------
    // Example 2: Simple Chat (helper method)
    // -------------------------------------------------------------------------
    Console.WriteLine("2. Simple Chat Example...");
    var response = await client.ChatAsync(
        message: "What is the capital of France?",
        model: CompactifAIModels.Llama31_8B_Slim,
        systemPrompt: "You are a helpful assistant. Be concise.");
    Console.WriteLine($"   Assistant: {response}");
    Console.WriteLine();

    // -------------------------------------------------------------------------
    // Example 3: Chat Completion (full control)
    // -------------------------------------------------------------------------
    Console.WriteLine("3. Chat Completion Example (full control)...");
    var chatRequest = new ChatCompletionRequest
    {
        Model = CompactifAIModels.Llama31_8B_Slim,
        Messages = new List<ChatMessage>
        {
            ChatMessage.System("You are a helpful assistant."),
            ChatMessage.User("Explain what .NET is in one sentence.")
        },
        MaxTokens = 100,
        Temperature = 0.7f
    };

    var chatResponse = await client.CreateChatCompletionAsync(chatRequest);
    var firstChoice = chatResponse.Choices?.FirstOrDefault();
    if (firstChoice != null)
    {
        Console.WriteLine($"   Assistant: {firstChoice.Message?.Content}");
        Console.WriteLine($"   Tokens used: {chatResponse.Usage?.TotalTokens}");
    }
    Console.WriteLine();

    // -------------------------------------------------------------------------
    // Example 4: Simple Text Completion (helper method)
    // -------------------------------------------------------------------------
    Console.WriteLine("4. Simple Text Completion Example...");
    var completedText = await client.CompleteAsync(
        prompt: "The benefits of using .NET for enterprise applications are:",
        model: CompactifAIModels.Llama31_8B_Slim,
        maxTokens: 100);
    Console.WriteLine($"   Completion: {completedText}");
    Console.WriteLine();

    // -------------------------------------------------------------------------
    // Example 5: Text Completion (full control)
    // -------------------------------------------------------------------------
    Console.WriteLine("5. Text Completion Example (full control)...");
    var completionRequest = new CompletionRequest
    {
        Model = CompactifAIModels.Llama31_8B_Slim,
        Prompt = "Write a haiku about programming:",
        MaxTokens = 50,
        Temperature = 0.8f
    };

    var completionResponse = await client.CreateCompletionAsync(completionRequest);
    var completionChoice = completionResponse.Choices?.FirstOrDefault();
    if (completionChoice != null)
    {
        Console.WriteLine($"   Result: {completionChoice.Text}");
    }
    Console.WriteLine();

    // -------------------------------------------------------------------------
    // Example 6: Audio Transcription (if you have an audio file)
    // -------------------------------------------------------------------------
    // Console.WriteLine("6. Audio Transcription Example...");
    // var audioFilePath = "path/to/your/audio.mp3";
    // if (File.Exists(audioFilePath))
    // {
    //     var transcription = await client.TranscribeFileAsync(
    //         filePath: audioFilePath,
    //         language: "en");
    //     Console.WriteLine($"   Transcription: {transcription}");
    // }

    Console.WriteLine("=== All examples completed successfully! ===");
}
catch (CompactifAIException ex)
{
    Console.WriteLine($"CompactifAI API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
