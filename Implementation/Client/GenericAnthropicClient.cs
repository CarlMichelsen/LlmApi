using Domain.Abstraction;
using Domain.Entity;
using Implementation.Map.Llm;
using Interface.Client;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace Implementation.Client;

public class GenericAnthropicClient(
    AnthropicClient anthropicClient) : IGenericLlmClient
{
    public async Task<Result<LlmResponse>> Prompt(LlmPromptDto llmPromptDto, ModelEntity modelEntity)
    {
        var mapper = new AnthropicPromptMapper(modelEntity);

        var anthropicPromptResult = mapper.Map(llmPromptDto);
        if (anthropicPromptResult.IsError)
        {
            return anthropicPromptResult.Error!;
        }

        var anthropicPrompt = anthropicPromptResult.Unwrap();
        var anthropicResponse = await anthropicClient.Prompt(anthropicPrompt);

        var llmResponseResult = mapper.Map(anthropicResponse);
        if (llmResponseResult.IsError)
        {
            return llmResponseResult.Error!;
        }

        return llmResponseResult.Unwrap();
    }

    public IAsyncEnumerable<Result<object>> PromptStream(LlmPromptDto llmPromptDto, ModelEntity modelEntity)
    {
        throw new NotImplementedException();
    }
}
