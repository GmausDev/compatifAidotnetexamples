# CompactifAI.Client Examples

[![NuGet](https://img.shields.io/nuget/v/CompactifAI.Client.svg)](https://www.nuget.org/packages/CompactifAI.Client)
[![.NET](https://img.shields.io/badge/.NET-8.0%2B-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This repository contains examples demonstrating how to use the [CompactifAI.Client](https://www.nuget.org/packages/CompactifAI.Client) NuGet package to integrate AI capabilities into your .NET applications.

## Overview

CompactifAI.Client is a .NET 8+ client library for the CompactifAI API. It provides easy-to-use interfaces for:

- **Chat Completions** - Conversational AI with system prompts
- **Text Completions** - Generate text continuations
- **Audio Transcription** - Convert speech to text
- **Model Management** - List and retrieve model information

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A CompactifAI API key ([Get one here](https://compactif.ai))

### Installation

```bash
dotnet add package CompactifAI.Client
```

### Quick Start

```csharp
using CompactifAI.Client;

// Create client
var client = new CompactifAIClient("your-api-key");

// Simple chat
var response = await client.ChatAsync(
    message: "What is the capital of France?",
    model: CompactifAIModels.Llama31_8B_Slim);

Console.WriteLine(response);
```

## Project Structure

```
CompactifAI.Example/           # Console application example
├── Program.cs                 # Usage examples
└── CompactifAI.Example.csproj

CompactifAI.Example.Tests/     # Unit tests
├── CompactifAIClientTests.cs  # Client and model tests
├── DependencyInjectionTests.cs # DI registration tests
└── CompactifAI.Example.Tests.csproj
```

## Examples

### Chat Completion

```csharp
using CompactifAI.Client;
using CompactifAI.Client.Models;

var client = new CompactifAIClient("your-api-key");

var request = new ChatCompletionRequest
{
    Model = CompactifAIModels.Llama31_8B_Slim,
    Messages = new List<ChatMessage>
    {
        ChatMessage.System("You are a helpful assistant."),
        ChatMessage.User("Explain quantum computing in simple terms.")
    },
    MaxTokens = 500,
    Temperature = 0.7f
};

var response = await client.CreateChatCompletionAsync(request);
Console.WriteLine(response.Choices.First().Message.Content);
```

### Text Completion

```csharp
var completion = await client.CompleteAsync(
    prompt: "The future of AI is",
    model: CompactifAIModels.Llama31_8B_Slim,
    maxTokens: 100);

Console.WriteLine(completion);
```

### Dependency Injection (ASP.NET Core)

```csharp
// In Program.cs or Startup.cs
services.AddCompactifAI(options =>
{
    options.ApiKey = Configuration["CompactifAI:ApiKey"];
    options.DefaultModel = CompactifAIModels.Llama31_8B_Slim;
});

// In your service or controller
public class MyService
{
    private readonly ICompactifAIClient _client;

    public MyService(ICompactifAIClient client)
    {
        _client = client;
    }

    public async Task<string> GetResponseAsync(string question)
    {
        return await _client.ChatAsync(question);
    }
}
```

### Audio Transcription

```csharp
var transcription = await client.TranscribeFileAsync(
    filePath: "audio.mp3",
    language: "en");

Console.WriteLine(transcription);
```

## Available Models

| Model | Description | Use Case |
|-------|-------------|----------|
| `DeepSeekR1_Slim` | Flagship reasoning model | Complex reasoning tasks |
| `Llama4Scout_Slim` | Long context model | Large document processing |
| `Llama33_70B_Slim` | High capability model | Complex tasks |
| `Llama31_8B_Slim` | Fast general purpose | Low latency applications |
| `MistralSmall31_Slim` | Efficient model | Complex tasks |
| `WhisperLargeV3` | Audio model | Speech-to-text |

## Running the Examples

### Console Example

```bash
# Set your API key
export COMPACTIFAI_API_KEY="your-api-key"

# Run the example
cd CompactifAI.Example
dotnet run
```

### Tests

```bash
cd CompactifAI.Example.Tests
dotnet test
```

## Configuration

### Environment Variables

| Variable | Description |
|----------|-------------|
| `COMPACTIFAI_API_KEY` | Your CompactifAI API key |
| `COMPACTIFAI_BASE_URL` | Optional: Custom API endpoint |

### appsettings.json

```json
{
  "CompactifAI": {
    "ApiKey": "your-api-key",
    "BaseUrl": "https://api.compactif.ai/v1",
    "DefaultModel": "llama-3.1-8b-slim",
    "TimeoutSeconds": 30
  }
}
```

## Error Handling

```csharp
try
{
    var response = await client.ChatAsync("Hello!");
}
catch (CompactifAIException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Response: {ex.ResponseBody}");
}
```

## Contributing

We welcome contributions! Please see our contributing guidelines:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Code of Conduct

This project follows the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).

## Support

- **Documentation**: [CompactifAI Docs](https://docs.compactif.ai)
- **Issues**: [GitHub Issues](https://github.com/yourusername/CompactifAI.Client/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/CompactifAI.Client/discussions)

## Related Projects

- [CompactifAI.Client](https://www.nuget.org/packages/CompactifAI.Client) - The official .NET client library
- [CompactifAI API](https://api.compactif.ai) - REST API documentation

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

<p align="center">
  Made with ❤️ by the CompactifAI community
</p>

<p align="center">
  <a href="https://compactif.ai">Website</a> •
  <a href="https://docs.compactif.ai">Documentation</a> •
  <a href="https://twitter.com/compactifai">Twitter</a>
</p>
