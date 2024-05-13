using Domain.Entity;
using Domain.Entity.Id;
using Implementation.Client;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTest.LargeLanguageModelClient.Fakes;

namespace UnitTest.LargeLanguageModelClient;

public class LargeLanguageModelClientTests
{
    private readonly FakeGenericLargeLanguageModelClient client;

    public LargeLanguageModelClientTests()
    {
        var loggerMock = new Mock<ILogger<GenericLargeLanguageModelClient>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        this.client = new FakeGenericLargeLanguageModelClient(loggerMock.Object, serviceProviderMock.Object);
    }

    [Theory]
    [InlineData(LlmProvider.Anthropic)]
    public async Task ShouldFinnishWithTotalUsageEvent(LlmProvider llmProvider)
    {
        // Arrange
        var prompt = this.MockPrompt(Enum.GetName(llmProvider)!);
        var model = this.MockModel(prompt.Model.ModelIdentifier, LlmProvider.Anthropic);
        LlmStreamEvent? lastEvent = default;

        // Act
        await foreach (var streamEvent in this.client.PromptStream(prompt, model))
        {
            lastEvent = streamEvent;
        }

        // Assert
        Assert.NotNull(lastEvent);
        Assert.Equal(LlmStreamEventType.TotalUsage, lastEvent.Type);
        Assert.IsType<LlmStreamTotalUsage>(lastEvent);
    }

    [Theory]
    [InlineData(LlmProvider.Anthropic)]
    public async Task ShouldFinnishWithTotalUsageEventEvenWhenCancelled(LlmProvider llmProvider)
    {
        // Arrange
        var prompt = this.MockPrompt(Enum.GetName(llmProvider)!);
        var model = this.MockModel(prompt.Model.ModelIdentifier, llmProvider);
        LlmStreamEvent? lastEvent = default;
        var source = new CancellationTokenSource();
        var max = 10;
        var counter = 0;

        // Act
        await foreach (var streamEvent in this.client.PromptStream(prompt, model, source.Token))
        {
            counter++;
            if (counter >= max)
            {
                source.Cancel();
            }

            lastEvent = streamEvent;
        }

        // Assert
        Assert.Equal(max + 1, counter); // +1 because there is supposed to be a TotalUsage event even after cancellation.
        Assert.NotNull(lastEvent);
        Assert.Equal(LlmStreamEventType.TotalUsage, lastEvent.Type);
        Assert.IsType<LlmStreamTotalUsage>(lastEvent);
    }

    private LlmPromptDto MockPrompt(string providerName) =>
        new LlmPromptDto(
            Model: new LlmPromptModelDto(
                ProviderName: providerName,
                ModelIdentifier: Guid.NewGuid()),
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
            ImageSupport = true,
            VideoSupport = false,
            JsonResponseOptimized = false,
            ModelDisplayName = "Fake Model",
            ModelDescription = "This model is literally fake.",
            LastUpdatedUtc = DateTime.UtcNow,
        };
    }
}
