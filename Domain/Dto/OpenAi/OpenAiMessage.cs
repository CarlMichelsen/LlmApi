using Domain.Dto.OpenAi.Content;

namespace Domain.Dto.OpenAi;

public record OpenAiMessage(
    string Role,
    List<OpenAiContent> Content);
