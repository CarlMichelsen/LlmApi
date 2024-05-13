using Domain.Dto.Anthropic.Response.Stream;
using Domain.Entity;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.Logging;

namespace Implementation.Map.Llm.Anthropic;

public class AnthropicStreamResponseMappingHandler(
    ILogger logger,
    ModelEntity model,
    AnthropicPromptMapper? optionalMapper = default)
{
    private readonly AnthropicPromptMapper mapper = optionalMapper ?? new AnthropicPromptMapper(model);

    private string? stopReason;

    private string? providerPromptIdentifier;
    private long inputTokens = 0;
    private long? outputTokens;
    private double outputTokensEstimate = 0;

    public LlmStreamEvent? AnthropicStreamMessageStart(AnthropicStreamMessageStart streamEvent)
    {
        var res = this.mapper.Map(streamEvent.Message).Unwrap();
        this.providerPromptIdentifier = res.ProviderPromptIdentifier;
        this.inputTokens = res.Usage.InputTokens;
        this.outputTokensEstimate = res.Usage.OutputTokens;

        return new LlmStreamMessageStart
        {
            Message = res,
        };
    }

    public LlmStreamEvent? AnthropicStreamContentBlockStart(AnthropicStreamContentBlockStart streamEvent)
    {
        return new LlmStreamContentStart
        {
            Index = streamEvent.Index,
        };
    }

    public LlmStreamEvent? AnthropicStreamContentBlockDelta(AnthropicStreamContentBlockDelta streamEvent)
    {
        this.outputTokensEstimate += 1.2;
        var llmContent = this.mapper.Map(streamEvent.Delta).Unwrap();
        return new LlmStreamContentDelta
        {
            Index = streamEvent.Index,
            Delta = llmContent,
        };
    }

    public LlmStreamEvent? AnthropicStreamContentBlockStop(AnthropicStreamContentBlockStop streamEvent)
    {
        return new LlmStreamContentStop
        {
            Index = streamEvent.Index,
        };
    }

    public LlmStreamEvent? AnthropicStreamMessageDelta(AnthropicStreamMessageDelta streamEvent)
    {
        this.stopReason = streamEvent.Delta.StopReason;
        this.outputTokens = streamEvent.Usage.OutputTokens;
        return default;
    }

    public LlmStreamEvent? AnthropicStreamMessageStop(AnthropicStreamMessageStop streamEvent)
    {
        if (this.stopReason is null)
        {
            return new LlmStreamError("No stop reason was given");
        }

        return new LlmStreamMessageStop
        {
            StopReason = this.stopReason,
        };
    }

    public LlmStreamEvent? CompleteStream()
    {
        if (string.IsNullOrWhiteSpace(this.providerPromptIdentifier))
        {
            logger.LogCritical("CompleteStream was fired without providerPromptIdentifier");
        }

        if (string.IsNullOrWhiteSpace(this.stopReason))
        {
            logger.LogCritical("CompleteStream was fired without stopReason");
        }

        return new LlmStreamTotalUsage
        {
            ProviderPromptIdentifier = this.providerPromptIdentifier ?? "NoIdentifier",
            InputTokens = this.inputTokens,
            OutputTokens = this.outputTokens,
            EstimatedOutputTokens = (long)this.outputTokensEstimate,
            StopReason = this.stopReason ?? "None",
        };
    }
}
