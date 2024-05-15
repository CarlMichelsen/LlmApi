namespace LargeLanguageModelClient.Dto.Prompt;

public record LlmPromptDto(
    Guid ModelIdentifier,
    string? SystemMessage,
    List<LlmPromptMessageDto> Messages);
