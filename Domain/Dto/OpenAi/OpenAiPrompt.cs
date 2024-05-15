using System.Text.Json.Serialization;
using Domain.Dto.OpenAi.Content;

namespace Domain.Dto.OpenAi;

public record OpenAiPrompt(
    string Model,
    List<OpenAiMessage> Messages,
    long MaxTokens,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    OpenAiStreamOptions? StreamOptions = default,
    bool Stream = false);
