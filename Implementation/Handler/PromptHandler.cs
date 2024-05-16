using Domain.Dto;
using Domain.Exception;
using Interface.Handler;
using Interface.Service;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;

namespace Implementation.Handler;

public class PromptHandler(
    ILargeLanguageModelService largeLanguageModelService) : IPromptHandler
{
    public async Task<ServiceResponse<LlmResponse>> Prompt(LlmPromptDto llmPromptDto, CancellationToken cancellationToken)
    {
        var promptResult = await largeLanguageModelService.Prompt(llmPromptDto, cancellationToken);
        if (promptResult.IsError)
        {
            return ServiceResponseFromException<LlmResponse>(promptResult.Error!);
        }

        return new ServiceResponse<LlmResponse>(promptResult.Unwrap());
    }

    private static ServiceResponse<T> ServiceResponseFromException<T>(Exception exception)
    {
        if (exception is SafeUserFeedbackException)
        {
            var safeUserFeedbackException = exception as SafeUserFeedbackException;
            return new ServiceResponse<T>($"{safeUserFeedbackException!.Message}, {string.Join(", ", safeUserFeedbackException!.Details)}");
        }

        return new ServiceResponse<T>("An error occured while handling the request");
    }
}
