using Domain.Abstraction;
using Domain.Entity.Id;
using Domain.Exception;
using Implementation.Client;
using Implementation.Validator;
using Interface.Repository;
using Interface.Service;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.Logging;

namespace Implementation.Service;

public class LargeLanguageModelService(
    ILogger<LargeLanguageModelService> logger,
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

    public async IAsyncEnumerable<LlmStreamEvent> PromptStream(LlmPromptDto llmPromptDto)
    {
        var validator = new LlmPromptDtoValidator();
        var validationResults = validator.Validate(llmPromptDto);
        if (!validationResults.IsValid)
        {
            var errorStrings = validationResults.Errors
                .Select(err => err.ErrorMessage)
                .ToArray();
            
            var joinedErrorStrings = string.Join(", ", errorStrings);
            yield return new LlmStreamError($"Invalid prompt {joinedErrorStrings}");
            yield break;
        }

        var modelResult = await llmModelRepository
            .GetModel(new ModelEntityId(llmPromptDto.Model.ModelIdentifier));
        if (modelResult.IsError)
        {
            logger.LogCritical(modelResult.Error!, "Failed to get model for prompt");
            yield return new LlmStreamError($"Invalid model identifier");
            yield break;
        }

        var stream = genericLargeLanguageModelClient
            .PromptStream(llmPromptDto, modelResult.Unwrap());
        await foreach (var item in stream)
        {
            yield return item; 
        }
    }
}
