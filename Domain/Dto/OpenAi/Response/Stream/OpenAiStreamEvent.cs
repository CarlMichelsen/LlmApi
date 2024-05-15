namespace Domain.Dto.OpenAi.Response.Stream;

public record OpenAiStreamEvent(
    string Id,
    string Object,
    long Created,
    string Model,
    string SystemFingerprint,
    List<OpenAiStreamChoice> Choices,
    OpenAiUsage? Usage = default);