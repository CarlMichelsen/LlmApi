using Domain.Dto.Anthropic;
using Domain.Dto.Anthropic.Content;
using Domain.Dto.Anthropic.Response.Stream;
using Implementation.Client;
using UnitTest.LargeLanguageModelClient.Fakes;

namespace UnitTest.LargeLanguageModelClient;

public class AnthropicStreamReaderTests
{
    private readonly AnthropicClient anthropicClient;

    public AnthropicStreamReaderTests()
    {
        this.anthropicClient = FakeAnthropicClientFactory.Create();
    }

    private AnthropicPrompt PlaceholderPrompt => new AnthropicPrompt(
            Model: "fake model",
            MaxTokens: 4000,
            System: "I'm a fake system message",
            Messages: new List<AnthropicMessage>
            {
                new AnthropicMessage(
                    "user",
                    Content: new List<AnthropicContent>
                    {
                        new AnthropicTextContent
                        {
                            Text = "Hello, World!",
                        },
                    }),
            });

    [Fact]
    public async Task ShouldReachTheLastStreamEventInMockStream()
    {
        // Arrange
        AnthropicStreamEvent? lastEvent = default;

        // Act
        await foreach (var streamEvent in this.anthropicClient.PromptStream(this.PlaceholderPrompt))
        {
            if (streamEvent.IsError)
            {
                break;
            }

            lastEvent = streamEvent.Unwrap();
        }

        // Assert
        Assert.NotNull(lastEvent);
        Assert.Equal("message_stop", lastEvent.Type);
        Assert.IsType<AnthropicStreamMessageStop>(lastEvent);
    }

    [Fact]
    public async Task ShouldStopGracefullyWhenCancelled()
    {
        // Arrange
        var max = 10;
        var counter = 0;
        var source = new CancellationTokenSource();

        // Act
        await foreach (var streamEvent in this.anthropicClient.PromptStream(this.PlaceholderPrompt, source.Token))
        {
            if (streamEvent.IsError)
            {
                break;
            }

            counter++;
            if (counter >= max)
            {
                source.Cancel();
            }
        }
        source.Dispose();

        // Assert
        Assert.Equal(max, counter);
    }
}
