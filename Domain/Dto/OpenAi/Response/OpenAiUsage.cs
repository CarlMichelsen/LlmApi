namespace Domain.Dto.OpenAi.Response;

public record OpenAiUsage(
    long PromptTokens,
    long CompletionTokens,
    long TotalTokens);