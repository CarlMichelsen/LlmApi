using Domain.Dto.Anthropic;
using Domain.Dto.Anthropic.Content;
using Domain.Dto.Anthropic.Response.Stream;
using UnitTest.LargeLanguageModelClient.Fakes;

namespace UnitTest.LargeLanguageModelClient;

public class AnthropicStreamReaderTests
{
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

    [Theory]
    [InlineData("anthropic-14-05-2024-10-05-13.txt")]
    [InlineData("anthropic-stream-response.txt")]
    public async Task ShouldReachTheLastStreamEventInMockStream(string testFileName)
    {
        // Arrange
        var anthropicClient = FakeAnthropicClientFactory.Create(testFileName);
        AnthropicStreamEvent? lastEvent = default;
        var error = false;

        // Act
        await foreach (var streamEvent in anthropicClient.PromptStream(this.PlaceholderPrompt, CancellationToken.None))
        {
            if (streamEvent.IsError)
            {
                error = true;
                break;
            }

            lastEvent = streamEvent.Unwrap();
        }

        // Assert
        Assert.False(error);
        Assert.NotNull(lastEvent);
        Assert.Equal("message_stop", lastEvent.Type);
        Assert.IsType<AnthropicStreamMessageStop>(lastEvent);
    }

    [Theory]
    [InlineData("anthropic-14-05-2024-10-05-13.txt")]
    [InlineData("anthropic-stream-response.txt")]
    public async Task ShouldStopGracefullyWhenCancelled(string testFileName)
    {
        // Arrange
        var anthropicClient = FakeAnthropicClientFactory.Create(testFileName);
        var max = 10;
        var counter = 0;
        var source = new CancellationTokenSource();

        // Act
        await foreach (var streamEvent in anthropicClient.PromptStream(this.PlaceholderPrompt, source.Token))
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
