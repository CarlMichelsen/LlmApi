namespace Domain.Dto.OpenAi.Response;

public record OpenAiChoice(
    int Index,
    OpenAiSimpleMessage Message,
    bool? Logprops,
    string FinnishReason);