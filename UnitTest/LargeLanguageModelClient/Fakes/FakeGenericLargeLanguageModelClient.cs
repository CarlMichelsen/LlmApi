using Domain.Entity;
using Implementation.Client;
using Interface.Client;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.LargeLanguageModelClient.Fakes;

public class FakeGenericLargeLanguageModelClient : GenericLargeLanguageModelClient
{
    public FakeGenericLargeLanguageModelClient(
        ILogger<GenericLargeLanguageModelClient> logger,
        IServiceProvider serviceProvider)
        : base(logger, serviceProvider)
    {
    }

    public override IGenericLlmClient GetGenericLlmClient(LlmProvider llmProvider)
    => llmProvider switch
        {
            LlmProvider.Anthropic => new GenericAnthropicClient(CreateFakeLogger<GenericAnthropicClient>(), FakeAnthropicClientFactory.Create()),
            _ => throw new NotImplementedException($"No fake client implemented for {nameof(llmProvider)}"),
        };
    
    private static ILogger<T> CreateFakeLogger<T>()
    {
        var loggerMock = new Mock<ILogger<T>>();
        return loggerMock.Object;
    }
}
