using System.Text.Json.Serialization;
using LargeLanguageModelClient.Dto.Prompt.Content;

namespace LargeLanguageModelClient.Dto.Prompt;

public record LlmPromptMessageDto(
    [property: JsonPropertyName("isUserMessage")] bool IsUserMessage,
    [property: JsonPropertyName("content")] List<LlmContent> Content);