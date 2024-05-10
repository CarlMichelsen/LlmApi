using Domain.Abstraction;
using Domain.Entity.Id;
using Domain.Exception;
using Implementation.Client;
using Implementation.Validator;
using Interface.Repository;
using Interface.Service;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace Implementation.Service;

public class LargeLanguageModelService(
    ILlmModelRepository llmModelRepository,
    GenericLargeLanguageModelClient genericLargeLanguageModelClient) : ILargeLanguageModelService
{
    public async Task<Result<LlmResponse>> Prompt(LlmPromptDto llmPromptDto)
    {
        var validator = new LlmPromptDtoValidator();
        var validationResults = validator.Validate(llmPromptDto);
        if (!validationResults.IsValid)
        {
            var errorStrings = validationResults.Errors
                .Select(err => err.ErrorMessage)
                .ToArray();
            
            return new SafeUserFeedbackException("Invalid prompt", errorStrings);
        }

        var modelResult = await llmModelRepository
            .GetModel(new ModelEntityId(llmPromptDto.Model.ModelIdentifier));
        if (modelResult.IsError)
        {
            return modelResult.Error!;
        }

        return await genericLargeLanguageModelClient
            .Prompt(llmPromptDto, modelResult.Unwrap());
    }

    public IAsyncEnumerable<Result<object>> PromptStream(LlmPromptDto llmPromptDto)
    {
        throw new NotImplementedException();
    }
}
