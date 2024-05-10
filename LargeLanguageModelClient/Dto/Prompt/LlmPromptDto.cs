namespace LargeLanguageModelClient.Dto.Prompt;

public record LlmPromptDto(
    LlmPromptModelDto Model,
    string? SystemMessage,
    List<LlmPromptMessageDto> Messages);
