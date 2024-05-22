using LargeLanguageModelClient.Dto.Response;

namespace LargeLanguageModelClient;

internal class LlmServiceResponse
{
    public required bool Ok { get; init; }

    public required LlmResponse? Data { get; init; }

    public required List<string> Errors { get; init; }
}
