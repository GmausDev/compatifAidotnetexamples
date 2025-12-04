using System.ClientModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using CompactifAI.Client;
using CompactifAI.Client.Models;
using OpenAI;
using OpenAI.Chat;

namespace CompactifAI.Benchmarks;

using CompactifAIChatMessage = Client.Models.ChatMessage;

/// <summary>
/// Benchmarks comparing SDK performance: CompactifAI.Client vs OpenAI NuGet package.
/// BOTH connect to the SAME CompactifAI API endpoint - we're measuring SDK overhead, not API speed.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ChatCompletionBenchmarks
{
    private CompactifAIClient _compactifAIClient = null!;
    private ChatClient _openAISDKClient = null!;

    private static readonly string CompactifAIModel = CompactifAIModels.Llama31_8B_Slim;

    private const string SimplePrompt = "What is 2 + 2? Reply with just the number.";
    private const string SystemPrompt = "You are a helpful assistant. Be extremely concise.";

    // Longer prompt to test serialization/deserialization overhead
    private const string LongerPrompt = """
        Analyze the following code snippet and provide a brief summary:

        public class Calculator {
            public int Add(int a, int b) => a + b;
            public int Subtract(int a, int b) => a - b;
            public int Multiply(int a, int b) => a * b;
            public double Divide(int a, int b) => b != 0 ? (double)a / b : throw new DivideByZeroException();
        }

        Reply in one sentence.
        """;

    [GlobalSetup]
    public void Setup()
    {
        var apiKey = Environment.GetEnvironmentVariable("COMPACTIFAI_API_KEY")
            ?? throw new InvalidOperationException("COMPACTIFAI_API_KEY environment variable is required");

        var baseUrl = Environment.GetEnvironmentVariable("COMPACTIFAI_BASE_URL")
            ?? "https://api.compactif.ai/v1";

        _compactifAIClient = new CompactifAIClient(apiKey, baseUrl);

        var openAIClientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(baseUrl)
        };

        _openAISDKClient = new ChatClient(
            model: CompactifAIModel,
            credential: new ApiKeyCredential(apiKey),
            options: openAIClientOptions
        );
    }

    // =========================================================================
    // SIMPLE CHAT COMPLETION - Basic request/response
    // =========================================================================

    [Benchmark(Description = "CompactifAI SDK - Simple Chat")]
    [BenchmarkCategory("Simple")]
    public async Task<string?> CompactifAI_SimpleChat()
    {
        var request = new ChatCompletionRequest
        {
            Model = CompactifAIModel,
            Messages =
            [
                CompactifAIChatMessage.System(SystemPrompt),
                CompactifAIChatMessage.User(SimplePrompt)
            ],
            MaxTokens = 10,
            Temperature = 0.0f
        };

        var response = await _compactifAIClient.CreateChatCompletionAsync(request);
        return response.Choices?.FirstOrDefault()?.Message?.Content;
    }

    [Benchmark(Description = "OpenAI SDK - Simple Chat")]
    [BenchmarkCategory("Simple")]
    public async Task<string?> OpenAI_SimpleChat()
    {
        List<OpenAI.Chat.ChatMessage> messages =
        [
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(SimplePrompt)
        ];

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 10,
            Temperature = 0.0f
        };

        var response = await _openAISDKClient.CompleteChatAsync(messages, options);
        return response.Value.Content.FirstOrDefault()?.Text;
    }

    // =========================================================================
    // LONGER PAYLOAD - Tests serialization overhead with larger messages
    // =========================================================================

    [Benchmark(Description = "CompactifAI SDK - Larger Payload")]
    [BenchmarkCategory("Payload")]
    public async Task<string?> CompactifAI_LargerPayload()
    {
        var request = new ChatCompletionRequest
        {
            Model = CompactifAIModel,
            Messages =
            [
                CompactifAIChatMessage.System("You are a code reviewer. Be concise."),
                CompactifAIChatMessage.User(LongerPrompt)
            ],
            MaxTokens = 50,
            Temperature = 0.0f
        };

        var response = await _compactifAIClient.CreateChatCompletionAsync(request);
        return response.Choices?.FirstOrDefault()?.Message?.Content;
    }

    [Benchmark(Description = "OpenAI SDK - Larger Payload")]
    [BenchmarkCategory("Payload")]
    public async Task<string?> OpenAI_LargerPayload()
    {
        List<OpenAI.Chat.ChatMessage> messages =
        [
            new SystemChatMessage("You are a code reviewer. Be concise."),
            new UserChatMessage(LongerPrompt)
        ];

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 50,
            Temperature = 0.0f
        };

        var response = await _openAISDKClient.CompleteChatAsync(messages, options);
        return response.Value.Content.FirstOrDefault()?.Text;
    }

    // =========================================================================
    // MULTI-TURN CONVERSATION - Tests message array serialization
    // =========================================================================

    [Benchmark(Description = "CompactifAI SDK - Multi-Turn (5 msgs)")]
    [BenchmarkCategory("MultiTurn")]
    public async Task<string?> CompactifAI_MultiTurn()
    {
        var request = new ChatCompletionRequest
        {
            Model = CompactifAIModel,
            Messages =
            [
                CompactifAIChatMessage.System(SystemPrompt),
                CompactifAIChatMessage.User("What is 1+1?"),
                CompactifAIChatMessage.Assistant("2"),
                CompactifAIChatMessage.User("What is 2+2?"),
                CompactifAIChatMessage.Assistant("4"),
                CompactifAIChatMessage.User("What is 3+3?")
            ],
            MaxTokens = 5,
            Temperature = 0.0f
        };

        var response = await _compactifAIClient.CreateChatCompletionAsync(request);
        return response.Choices?.FirstOrDefault()?.Message?.Content;
    }

    [Benchmark(Description = "OpenAI SDK - Multi-Turn (5 msgs)")]
    [BenchmarkCategory("MultiTurn")]
    public async Task<string?> OpenAI_MultiTurn()
    {
        List<OpenAI.Chat.ChatMessage> messages =
        [
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage("What is 1+1?"),
            new AssistantChatMessage("2"),
            new UserChatMessage("What is 2+2?"),
            new AssistantChatMessage("4"),
            new UserChatMessage("What is 3+3?")
        ];

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 5,
            Temperature = 0.0f
        };

        var response = await _openAISDKClient.CompleteChatAsync(messages, options);
        return response.Value.Content.FirstOrDefault()?.Text;
    }

    // =========================================================================
    // CONCURRENT REQUESTS - Tests connection pooling and parallel handling
    // =========================================================================

    [Benchmark(Description = "CompactifAI SDK - 3 Concurrent")]
    [BenchmarkCategory("Concurrent")]
    public async Task<string[]> CompactifAI_Concurrent()
    {
        var tasks = Enumerable.Range(1, 3).Select(async i =>
        {
            var request = new ChatCompletionRequest
            {
                Model = CompactifAIModel,
                Messages =
                [
                    CompactifAIChatMessage.System(SystemPrompt),
                    CompactifAIChatMessage.User($"What is {i} + {i}? Reply with just the number.")
                ],
                MaxTokens = 5,
                Temperature = 0.0f
            };

            var response = await _compactifAIClient.CreateChatCompletionAsync(request);
            return response.Choices?.FirstOrDefault()?.Message?.Content ?? "";
        });

        return await Task.WhenAll(tasks);
    }

    [Benchmark(Description = "OpenAI SDK - 3 Concurrent")]
    [BenchmarkCategory("Concurrent")]
    public async Task<string[]> OpenAI_Concurrent()
    {
        var tasks = Enumerable.Range(1, 3).Select(async i =>
        {
            List<OpenAI.Chat.ChatMessage> messages =
            [
                new SystemChatMessage(SystemPrompt),
                new UserChatMessage($"What is {i} + {i}? Reply with just the number.")
            ];

            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 5,
                Temperature = 0.0f
            };

            var response = await _openAISDKClient.CompleteChatAsync(messages, options);
            return response.Value.Content.FirstOrDefault()?.Text ?? "";
        });

        return await Task.WhenAll(tasks);
    }

    // =========================================================================
    // HELPER METHOD COMPARISON - Tests SDK convenience methods
    // =========================================================================

    [Benchmark(Description = "CompactifAI SDK - Helper Method")]
    [BenchmarkCategory("Helper")]
    public async Task<string?> CompactifAI_HelperMethod()
    {
        return await _compactifAIClient.ChatAsync(
            message: SimplePrompt,
            model: CompactifAIModel,
            systemPrompt: SystemPrompt);
    }

    [Benchmark(Description = "OpenAI SDK - Direct API")]
    [BenchmarkCategory("Helper")]
    public async Task<string?> OpenAI_DirectAPI()
    {
        // OpenAI SDK doesn't have a simple ChatAsync helper, so we use the full API
        var response = await _openAISDKClient.CompleteChatAsync(
            [
                new SystemChatMessage(SystemPrompt),
                new UserChatMessage(SimplePrompt)
            ]);

        return response.Value.Content.FirstOrDefault()?.Text;
    }
}
