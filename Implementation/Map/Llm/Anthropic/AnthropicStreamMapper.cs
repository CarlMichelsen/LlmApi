using System.Runtime.CompilerServices;
using Domain.Abstraction;
using Domain.Dto.Anthropic.Response.Stream;
using Domain.Entity;
using LargeLanguageModelClient.Dto.Response.Stream;
using Microsoft.Extensions.Logging;

namespace Implementation.Map.Llm.Anthropic;

public class AnthropicStreamMapper(ILogger logger, ModelEntity model)
{
    public async IAsyncEnumerable<LlmStreamEvent> MapToLlmStream(
        IAsyncEnumerable<Result<AnthropicStreamEvent>> stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var handler = new AnthropicStreamResponseMappingHandler(logger, model);
        await foreach (var anthropicStreamEventResult in stream)
        {
            if (anthropicStreamEventResult.IsError)
            {
                yield return LlmStreamMapper.HandleSafeUserFeedback(anthropicStreamEventResult.Error!, logger);
                break;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var anthropicStreamEvent = anthropicStreamEventResult.Unwrap();
            var antropicEventType = anthropicStreamEvent.Type;
            LlmStreamEvent? llmStreamEvent = antropicEventType switch
            {
                "error" => handler.AnthropicStreamError((AnthropicStreamError)anthropicStreamEvent),
                "message_start" => handler.AnthropicStreamMessageStart((AnthropicStreamMessageStart)anthropicStreamEvent),
                "content_block_start" => handler.AnthropicStreamContentBlockStart((AnthropicStreamContentBlockStart)anthropicStreamEvent),
                "content_block_delta" => handler.AnthropicStreamContentBlockDelta((AnthropicStreamContentBlockDelta)anthropicStreamEvent),
                "content_block_stop" => handler.AnthropicStreamContentBlockStop((AnthropicStreamContentBlockStop)anthropicStreamEvent),
                "message_delta" => handler.AnthropicStreamMessageDelta((AnthropicStreamMessageDelta)anthropicStreamEvent),
                "message_stop" => handler.AnthropicStreamMessageStop((AnthropicStreamMessageStop)anthropicStreamEvent),
                _ => throw new NotImplementedException($"Unsupported anthropic event in mapper \"{antropicEventType}\""),
            };

            if (llmStreamEvent is not null)
            {
                yield return llmStreamEvent;
            }
        }

        var finalEvent = handler.CompleteStream();
        if (finalEvent is not null)
        {
            yield return finalEvent;
        }
    }
}
