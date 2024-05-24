using LargeLanguageModelClient.Dto;
using LargeLanguageModelClient.Dto.Model;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;

namespace LargeLanguageModelClient;

public interface ILargeLanguageModelClient
{
    Task<ServiceResponse<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken);

    IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        CancellationToken cancellationToken);
    
    Task<ServiceResponse<LlmModelDto>> GetModel(Guid modelId);

    Task<ServiceResponse<List<LlmModelDto>>> GetAllModels();
}