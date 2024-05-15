using System.Runtime.CompilerServices;
using Domain.Abstraction;
using Domain.Dto.OpenAi.Response.Stream;
using Domain.Entity;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.Logging;

namespace Implementation.Map.Llm.OpenAi;

public class OpenAiStreamMapper(
    ILogger logger,
    ModelEntity model,
    LlmPromptDto llmPromptDto)
{
    private readonly OpenAiStreamResponseMappingHandler handler = new(model, llmPromptDto);
    private bool receivedFirstEvent = false;

    public async IAsyncEnumerable<LlmStreamEvent> MapToLlmStream(
        IAsyncEnumerable<Result<OpenAiStreamEvent>> stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var openAiStreamEventResult in stream)
        {
            if (openAiStreamEventResult.IsError)
            {
                yield return LlmStreamMapper.HandleSafeUserFeedback(
                    openAiStreamEventResult.Error!,
                    logger);
                
                yield return this.handler.CreateTotalUsageFromCurrentInformation(true);
                yield break;
            }

            var openAiStreamEvent = openAiStreamEventResult.Unwrap();
            if (!this.receivedFirstEvent)
            {
                foreach (var streamEvent in this.handler.MapInitialMessage(openAiStreamEvent))
                {
                    yield return streamEvent;
                }
                
                this.receivedFirstEvent = true;
            }

            foreach (var streamEvent in this.HandleContentAndEnding(openAiStreamEvent, cancellationToken))
            {
                yield return streamEvent;
                if (cancellationToken.IsCancellationRequested && streamEvent is LlmStreamTotalUsage)
                {
                    yield break;
                }
            }
        }
    }

    private IEnumerable<LlmStreamEvent> HandleContentAndEnding(
        OpenAiStreamEvent openAiStreamEvent,
        CancellationToken cancellationToken)
    {
        if (openAiStreamEvent.Usage is null)
        {
            var contentDelta = this.handler.MapContentDelta(openAiStreamEvent);
            if (contentDelta is not null)
            {
                yield return contentDelta;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                yield return this.handler.CreateTotalUsageFromCurrentInformation(true, "cancelled");
                yield break;
            }
        }
        else
        {
            // This is the last event.
            foreach (var streamEvent in this.handler.MapClosingEvents(openAiStreamEvent))
            {
                yield return streamEvent;
            }

            yield break;
        }
    }
}
