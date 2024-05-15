using Domain.Dto.OpenAi.Response;

namespace Domain.Dto.OpenAi;

public record OpenAiResponse(
    string Id,
    string Object,
    ulong Created,
    string Model,
    string SystemFingerprint,
    List<OpenAiChoice> Choices,
    OpenAiUsage Usage);
