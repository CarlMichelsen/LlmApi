using Domain.Entity;
using Implementation.Client;
using Interface.Client;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.LargeLanguageModelClient.Fakes;

public class FakeGenericLargeLanguageModelClient : GenericLargeLanguageModelClient
{
    private readonly string testFileName;

    public FakeGenericLargeLanguageModelClient(
        IServiceProvider serviceProvider,
        string testFileName)
        : base(serviceProvider)
    {
        this.testFileName = testFileName;
    }

    public override IGenericLlmClient GetGenericLlmClient(LlmProvider llmProvider)
    => llmProvider switch
        {
            LlmProvider.Anthropic => new GenericAnthropicClient(CreateFakeLogger<GenericAnthropicClient>(), FakeAnthropicClientFactory.Create(this.testFileName)),
            LlmProvider.OpenAi => new GenericOpenAiClient(CreateFakeLogger<GenericOpenAiClient>(), FakeOpenAiClientFactory.Create(this.testFileName)),
            _ => throw new NotImplementedException($"No fake client implemented for {nameof(llmProvider)}"),
        };
    
    private static ILogger<T> CreateFakeLogger<T>()
    {
        var loggerMock = new Mock<ILogger<T>>();
        return loggerMock.Object;
    }
}
