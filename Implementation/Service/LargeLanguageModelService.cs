using System.Runtime.CompilerServices;
using Domain.Abstraction;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Implementation.Client;
using Implementation.Validator;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;

namespace Implementation.Service;

public class LargeLanguageModelService(
    GenericLargeLanguageModelClient genericLargeLanguageModelClient)
{
    public async Task<Result<LlmResponse>> Prompt(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        CancellationToken cancellationToken)
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

        return await genericLargeLanguageModelClient
            .Prompt(llmPromptDto, modelEntity, cancellationToken);
    }

    public async IAsyncEnumerable<LlmStreamEvent> PromptStream(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        [EnumeratorCancellation] CancellationToken cancellationToken)
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

        var stream = genericLargeLanguageModelClient
            .PromptStream(llmPromptDto, modelEntity, cancellationToken);
        await foreach (var item in stream)
        {
            yield return item; 
        }
    }
}
