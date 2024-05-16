using Domain.Entity;
using Domain.Entity.Id;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Moq;
using UnitTest.LargeLanguageModelClient.Fakes;

namespace UnitTest.LargeLanguageModelClient;

public class GenericLargeLanguageModelClientTests
{
    private readonly Mock<IServiceProvider> mockServiceProvider;

    public GenericLargeLanguageModelClientTests()
    {
        this.mockServiceProvider = new();
    }

    [Theory]
    [InlineData(LlmProvider.Anthropic, "anthropic-14-05-2024-10-05-13.txt")]
    [InlineData(LlmProvider.Anthropic, "anthropic-stream-response.txt")]
    [InlineData(LlmProvider.OpenAi, "openai-14-05-2024-09-45-37.txt")]
    [InlineData(LlmProvider.OpenAi, "openai-14-05-2024-10-21-53.txt")]
    public async Task ShouldFinnishWithTotalUsageEvent(LlmProvider llmProvider, string testFileName)
    {
        // Arrange
        var client = new FakeGenericLargeLanguageModelClient(this.mockServiceProvider.Object, testFileName);
        var prompt = this.MockPrompt();
        var model = this.MockModel(prompt.ModelIdentifier, llmProvider);
        LlmStreamEvent? lastEvent = default;

        // Act
        await foreach (var streamEvent in client.PromptStream(prompt, model, CancellationToken.None))
        {
            lastEvent = streamEvent;
        }

        // Assert
        Assert.NotNull(lastEvent);
        Assert.Equal(LlmStreamEventType.TotalUsage, lastEvent.Type);
        Assert.IsType<LlmStreamTotalUsage>(lastEvent);
    }

    [Theory]
    [InlineData(LlmProvider.Anthropic, "anthropic-with-error-14-05-2024-10-43-56.txt")]
    [InlineData(LlmProvider.OpenAi, "openai-with-error-15-05-2024-15-05-28.txt")]
    public async Task ShouldGracefullyHandleErrors(LlmProvider llmProvider, string testFileName)
    {
        // Arrange
        var client = new FakeGenericLargeLanguageModelClient(this.mockServiceProvider.Object, testFileName);
        var prompt = this.MockPrompt();
        var model = this.MockModel(prompt.ModelIdentifier, llmProvider);
        var allEvents = new List<LlmStreamEvent>();

        // Act
        await foreach (var streamEvent in client.PromptStream(prompt, model, CancellationToken.None))
        {
            allEvents.Add(streamEvent);
        }

        // Arrange
        Assert.NotEmpty(allEvents);
        Assert.IsType<LlmStreamError>(allEvents[^2]);
        Assert.IsType<LlmStreamTotalUsage>(allEvents[^1]);
        Assert.Equal(LlmStreamEventType.TotalUsage, allEvents[^1].Type);
    }

    [Theory]
    [InlineData(LlmProvider.Anthropic, "anthropic-14-05-2024-10-05-13.txt")]
    [InlineData(LlmProvider.Anthropic, "anthropic-stream-response.txt")]
    [InlineData(LlmProvider.OpenAi, "openai-14-05-2024-09-45-37.txt")]
    [InlineData(LlmProvider.OpenAi, "openai-14-05-2024-10-21-53.txt")]
    public async Task ShouldFinnishWithTotalUsageEventEvenWhenCancelled(LlmProvider llmProvider, string testFileName)
    {
        // Arrange
        var client = new FakeGenericLargeLanguageModelClient(this.mockServiceProvider.Object, testFileName);
        var prompt = this.MockPrompt();
        var model = this.MockModel(prompt.ModelIdentifier, llmProvider);
        var allEvents = new List<LlmStreamEvent>();
        var source = new CancellationTokenSource();
        var max = 5;
        var counter = 0;

        // Act
        await foreach (var streamEvent in client.PromptStream(prompt, model, source.Token))
        {
            counter++;
            if (counter >= max)
            {
                source.Cancel();
            }

            allEvents.Add(streamEvent);
        }
        
        source.Dispose();

        // Assert
        Assert.Equal(max + 1, counter); // +1 because there is supposed to be a TotalUsage event after cancellation.
        Assert.NotNull(allEvents[^1]);
        Assert.Equal(LlmStreamEventType.TotalUsage, allEvents[^1].Type);
        Assert.IsNotType<LlmStreamError>(allEvents[^2]);
        Assert.IsType<LlmStreamTotalUsage>(allEvents[^1]);
    }

    private LlmPromptDto MockPrompt() =>
        new LlmPromptDto(
            ModelIdentifier: Guid.NewGuid(),
            SystemMessage: "I'm a fake system message",
            Messages: new List<LlmPromptMessageDto>
            {
                new LlmPromptMessageDto(
                    IsUserMessage: true,
                    Content: new List<LlmContent>
                    {
                        new LlmTextContent
                        {
                            Text = "Hello, World!",
                        },
                    }),
            });

    private ModelEntity MockModel(Guid identifier, LlmProvider provider)
    {
        return new ModelEntity
        {
            Id = new ModelEntityId(identifier),
            Available = true,
            Provider = provider,
            Price = new PriceEntity
            {
                Id = new PriceEntityId(Guid.NewGuid()),
                ModelId = new ModelEntityId(identifier),
                MillionInputTokenPrice = 200,
                MillionOutputTokenPrice = 400,
            },
            ModelIdentifierName = "fake-model-identifier-name",
            MaxTokenCount = 4000,
            ContextTokenCount = 4000,
            ImageSupport = true,
            VideoSupport = false,
            JsonResponseOptimized = false,
            ModelDisplayName = "Fake Model",
            ModelDescription = "This model is literally fake.",
            LastUpdatedUtc = DateTime.UtcNow,
        };
    }
}
