using LargeLanguageModelClient.Dto.Prompt;

namespace LargeLanguageModelClient.Dto.Response;

public record LlmResponse(
    string ProviderPromptIdentifier,
    Guid ModelId,
    string ModelIdentifierName,
    LlmPromptMessageDto Message,
    LlmUsage Usage,
    string? StopReason,
    string? DetailedModelIdentifierName = default);