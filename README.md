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

CompactifAI.Benchmarks/        # Performance benchmarks
├── Program.cs                 # Benchmark runner
├── ChatCompletionBenchmarks.cs # SDK comparison benchmarks
└── CompactifAI.Benchmarks.csproj
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

## Performance Benchmarks

We benchmark CompactifAI.Client against the official [OpenAI .NET SDK](https://www.nuget.org/packages/OpenAI) to ensure optimal performance. Both SDKs connect to the **same CompactifAI API endpoint**, measuring pure SDK overhead.

### Benchmark Environment

- **Runtime**: .NET 10.0, Arm64 RyuJIT AdvSIMD
- **Hardware**: Apple M3 Pro, 12 cores
- **Tool**: [BenchmarkDotNet](https://benchmarkdotnet.org/) v0.14.0
- **Version**: CompactifAI.Client v1.1.0

### Results Summary

| Benchmark | CompactifAI.Client | OpenAI SDK | Memory Savings |
|-----------|-------------------|------------|----------------|
| Multi-Turn (5 msgs) | **67.92 ms** / 9.27 KB | 68.29 ms / 15.13 KB | **39% less** |
| Simple Chat | 72.43 ms / 9.01 KB | 71.01 ms / 55.56 KB | **84% less** |
| Helper Method | 71.60 ms / 8.97 KB | 77.74 ms / 14.16 KB | **37% less** |
| 3 Concurrent Requests | **78.29 ms** / 27.27 KB | 78.81 ms / 42.17 KB | **35% less** |
| Larger Payload | 196.21 ms / 10.8 KB | 196.76 ms / 15.5 KB | **30% less** |

### Key Findings

| Metric | Result |
|--------|--------|
| **Memory Efficiency** | CompactifAI.Client uses **35-84% less memory** |
| **Multi-Turn Performance** | CompactifAI.Client is faster for conversation history |
| **Concurrent Performance** | CompactifAI.Client handles parallel requests better |
| **API Simplicity** | One-liner helper methods (`ChatAsync`, `CompleteAsync`) |

### Why Choose CompactifAI.Client?

- **Significantly lower memory footprint** - Up to 84% less allocations
- **Optimized for conversations** - Faster multi-turn message handling
- **Simpler API** - Helper methods reduce boilerplate
- **Built-in DI** - `services.AddCompactifAI()` for ASP.NET Core
- **CompactifAI-optimized** - Model constants, audio transcription support

### Run Benchmarks Yourself

```bash
cd CompactifAI.Benchmarks
export COMPACTIFAI_API_KEY="your-api-key"
dotnet run -c Release
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
</p>
