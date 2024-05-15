namespace Domain.Dto.OpenAi.Response;

public record OpenAiSimpleMessage(
    string? Role,
    string Content);