namespace LargeLanguageModelClient.Dto.Prompt;

public record LlmPromptModelDto(
    string ProviderName,
    Guid ModelIdentifier);