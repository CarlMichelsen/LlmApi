using System.Runtime.CompilerServices;
using System.Text.Json;
using Domain.Abstraction;
using Domain.Dto.OpenAi.Response.Stream;
using Domain.Exception;
using Interface.Json;

namespace Implementation.Json.Reader;

public class OpenAiStreamReader(
    IStreamLineReader streamLineReader)
{
    public const string Delimiter = "data: ";
    public const string DoneString = "[DONE]";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public async IAsyncEnumerable<Result<OpenAiStreamEvent>> Read(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var lineReader = streamLineReader.ReadLine(stream, cancellationToken);

        await foreach (var line in lineReader)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var split = line.Split(Delimiter);
            if (split.Length != 2)
            {
                yield return new SafeUserFeedbackException("OpenAi stream chunk parsing had unexpected length");
                yield break;
            }

            var data = split.Last();

            if (data == DoneString)
            {
                yield break;
            }

            yield return this.HandleDeserialization(data);
        }
    }

    private Result<OpenAiStreamEvent> HandleDeserialization(string data)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<OpenAiStreamEvent>(data, Options);
            if (deserialized is null)
            {
                return new SafeUserFeedbackException("OpenAi stream chunk parsing failed");
            }

            return deserialized;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}
