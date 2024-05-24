using System.Text.Json.Serialization;

namespace LargeLanguageModelClient.Dto.Prompt;

public record LlmPromptDto(
    [property: JsonPropertyName("modelIdentifier")] Guid ModelIdentifier,
    [property: JsonPropertyName("systemMessage")] string? SystemMessage,
    [property: JsonPropertyName("messages")] List<LlmPromptMessageDto> Messages);
