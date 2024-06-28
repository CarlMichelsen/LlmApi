using Domain.Dto.OpenAi;
using Domain.Dto.OpenAi.Content;
using Domain.Dto.OpenAi.Response.Stream;
using UnitTest.LargeLanguageModelClient.Fakes;

namespace UnitTest.LargeLanguageModelClient;

public class OpenAiStreamReaderTests
{
    private OpenAiPrompt PlaceholderPrompt => new OpenAiPrompt(
            Model: "gpt-4o",
            Messages:
            [
                new OpenAiMessage(
                    "user",
                    new List<OpenAiContent>
                    {
                        new OpenAiTextContent
                        {
                            Text = "I'm a placeholder prompt, i don't actually do anything",
                        },
                    }),
            ],
            4000);
    
    [Theory]
    [InlineData("openai-14-05-2024-09-45-37.txt")]
    [InlineData("openai-14-05-2024-10-21-53.txt")]
    public async Task TheLastEventInMockStreamShouldHaveUsageData(string testFileName)
    {
        // Arrange
        var openAiClient = FakeOpenAiClientFactory.Create(testFileName);
        OpenAiStreamEvent? lastEvent = default;
        var error = false;

        // Act
        await foreach (var streamEvent in openAiClient.PromptStream(this.PlaceholderPrompt, CancellationToken.None))
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
        Assert.NotNull(lastEvent.Usage);
        Assert.True(lastEvent.Usage.PromptTokens > 0);
        Assert.True(lastEvent.Usage.CompletionTokens > 0);
        Assert.True(lastEvent.Usage.TotalTokens > 0);
        Assert.Equal(lastEvent.Usage.PromptTokens + lastEvent.Usage.CompletionTokens, lastEvent.Usage.TotalTokens);
    }

    [Theory]
    [InlineData("openai-14-05-2024-09-45-37.txt")]
    [InlineData("openai-14-05-2024-10-21-53.txt")]
    public async Task ShouldStopGracefullyWhenCancelled(string testFileName)
    {
        // Arrange
        var openAiClient = FakeOpenAiClientFactory.Create(testFileName);
        var max = 10;
        var counter = 0;
        var source = new CancellationTokenSource();

        // Act
        await foreach (var streamEvent in openAiClient.PromptStream(this.PlaceholderPrompt, source.Token))
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
