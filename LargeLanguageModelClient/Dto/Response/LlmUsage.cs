namespace LargeLanguageModelClient.Dto.Response;

public record LlmUsage(
    long InputTokens,
    long OutputTokens);
