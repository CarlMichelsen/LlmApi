using System.Runtime.CompilerServices;
using System.Text.Json;
using Domain.Abstraction;
using Domain.Dto.Anthropic.Response.Stream;
using Domain.Exception;
using Interface.Json;

namespace Implementation.Json.Reader;

public class AnthropicStreamHandler(
    IStreamLineReader streamLineReader)
{
    private const string LineTypeEvent = "event";
    private const string LineTypeData = "data";

    private static readonly List<string> IgnoredEvents = new()
    {
        "ping",
    };

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters =
        {
            new AnthropicContentConverter(),
        },
    };

    public async IAsyncEnumerable<Result<AnthropicStreamEvent>> ReadLine(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var lineReader = streamLineReader.ReadLine(stream, cancellationToken);

        string? eventName = default;
        await foreach (var line in lineReader)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var lineTypeSplitResult = LineTypeSplit(line);
            if (lineTypeSplitResult.IsError)
            {
                yield return lineTypeSplitResult.Error!;
                yield break;
            }

            var lineTypeSplit = lineTypeSplitResult.Unwrap();
            if (lineTypeSplit.LineType == LineTypeEvent)
            {
                eventName = lineTypeSplit.Data;
            }
            else if (lineTypeSplit.LineType == LineTypeData && !string.IsNullOrWhiteSpace(eventName))
            {
                var tempEventName = eventName;
                eventName = default;

                if (IgnoredEvents.Contains(tempEventName))
                {
                    continue;
                }

                yield return ParseStreamEvent(tempEventName, lineTypeSplit.Data);
            }
            else
            {
                yield return new SafeUserFeedbackException(
                    $"LineType \"{lineTypeSplit.LineType}\" is not known in anthropic stream handler");
                yield break;
            }
        }
    }

    private static Result<AnthropicStreamEvent> ParseStreamEvent(string eventName, string data)
        => eventName switch
        {
            "message_start" => ParseStreamEvent<AnthropicStreamMessageStart>(data),
            "content_block_start" => ParseStreamEvent<AnthropicStreamContentBlockStart>(data),
            "content_block_delta" => ParseStreamEvent<AnthropicStreamContentBlockDelta>(data),
            "content_block_stop" => ParseStreamEvent<AnthropicStreamContentBlockStop>(data),
            "message_delta" => ParseStreamEvent<AnthropicStreamMessageDelta>(data),
            "message_stop" => ParseStreamEvent<AnthropicStreamMessageStop>(data),
            _ => new SafeUserFeedbackException($"LineType \"{eventName}\" is not known in anthropic stream handler"),
        };

    private static Result<AnthropicStreamEvent> ParseStreamEvent<T>(string data)
        where T : AnthropicStreamEvent
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<T>(data, Options);
            if (deserialized is null)
            {
                return new SafeUserFeedbackException("Failed to parse a section of the anthropic stream");
            }

            return deserialized;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private static Result<(string LineType, string Data)> LineTypeSplit(string line)
    {
        var split = line.Split(": ");
        if (split.Length < 2)
        {
            return new SafeUserFeedbackException("Stream parse error");
        }

        return (split[0], split[1]);
    }
}
