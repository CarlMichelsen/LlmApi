using LargeLanguageModelClient.Dto;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace Interface.Handler;

public interface IPromptHandler
{
    Task<ServiceResponse<LlmResponse>> Prompt(LlmPromptDto llmPromptDto, CancellationToken cancellationToken);
}
