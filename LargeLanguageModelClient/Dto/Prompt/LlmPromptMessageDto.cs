using LargeLanguageModelClient.Dto.Prompt.Content;

namespace LargeLanguageModelClient.Dto.Prompt;

public record LlmPromptMessageDto(
    bool IsUserMessage,
    List<LlmContent> Content);