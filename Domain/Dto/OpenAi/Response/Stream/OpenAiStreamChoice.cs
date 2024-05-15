namespace Domain.Dto.OpenAi.Response.Stream;

public record OpenAiStreamChoice(
    int Index,
    OpenAiSimpleMessage Delta,
    bool? Logprobs,
    string FinnishReason);
