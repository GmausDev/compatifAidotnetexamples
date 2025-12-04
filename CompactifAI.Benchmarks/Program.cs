using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using CompactifAI.Benchmarks;

// =============================================================================
// SDK Performance Benchmarks: CompactifAI.Client vs OpenAI NuGet
// =============================================================================
// This benchmark compares the SDK overhead between:
//   1. CompactifAI.Client NuGet package
//   2. OpenAI NuGet package (configured to use CompactifAI endpoint)
//
// BOTH SDKs connect to the SAME CompactifAI API - we measure SDK performance,
// not API provider differences.
//
// Required environment variables:
//   - COMPACTIFAI_API_KEY: Your CompactifAI API key
//   - COMPACTIFAI_BASE_URL: (Optional) Defaults to https://api.compactif.ai/v1
//
// Usage:
//   dotnet run -c Release
//
// =============================================================================

Console.WriteLine("=== SDK Performance Benchmarks ===");
Console.WriteLine("CompactifAI.Client SDK vs OpenAI NuGet SDK\n");
Console.WriteLine("Both SDKs connect to CompactifAI API - measuring SDK overhead only.\n");

// Validate environment variables
var compactifAIKey = Environment.GetEnvironmentVariable("COMPACTIFAI_API_KEY");
var baseUrl = Environment.GetEnvironmentVariable("COMPACTIFAI_BASE_URL") ?? "https://api.compactif.ai/v1";

if (string.IsNullOrEmpty(compactifAIKey))
{
    Console.WriteLine("ERROR: COMPACTIFAI_API_KEY environment variable is not set.");
    Console.WriteLine("Please set it before running benchmarks.");
    return 1;
}

Console.WriteLine($"API Key: {compactifAIKey[..8]}...");
Console.WriteLine($"Base URL: {baseUrl}\n");
Console.WriteLine("Starting benchmarks...\n");

#if DEBUG
Console.WriteLine("WARNING: Running in DEBUG mode. For accurate benchmarks, run in RELEASE mode:");
Console.WriteLine("  dotnet run -c Release\n");
var config = new DebugInProcessConfig();
BenchmarkRunner.Run<ChatCompletionBenchmarks>(config);
#else
BenchmarkRunner.Run<ChatCompletionBenchmarks>();
#endif

return 0;
