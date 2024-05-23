using LargeLanguageModelClient.Dto.Model;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;

namespace LargeLanguageModelClient;

public interface ILargeLanguageModelClient
{
    Task<LlmResponse> Prompt(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken);

    IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken);
    
    Task<LlmModelDto?> GetModel(Guid modelId);

    Task<List<LlmModelDto>> GetAllModels();
}