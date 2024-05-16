using System.Text;
using Domain.Dto.OpenAi.Response.Stream;
using Domain.Entity;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;

namespace Implementation.Map.Llm.OpenAi;

public class OpenAiStreamResponseMappingHandler(
    ModelEntity model,
    LlmPromptDto llmPromptDto)
{
    private long inputTokens = 0;
    private long outputTokens = 0;
    private double estimatedOutputTokens = 0;
    private string lastKnownProviderPromptIdentifier = "No prompt data received";

    public double EstimatePromptTokens()
    {
        var sb = new StringBuilder(llmPromptDto.SystemMessage);
        var promptTextContentList = llmPromptDto.Messages
            .SelectMany(x => x.Content)
            .Where(x => x.Type == LargeLanguageModelClient.LlmContentType.Text)
            .Select(x => (x as LlmTextContent)!.Text)
            .ToList();
        
        foreach (var text in promptTextContentList)
        {
            sb.AppendLine(text);
        }

        var promptTextContent = sb.ToString();
        return promptTextContent.Split(' ').Length * 0.8;
    }

    public List<LlmStreamEvent> MapInitialMessage(
        OpenAiStreamEvent streamEvent)
    {
        this.lastKnownProviderPromptIdentifier = streamEvent.Id;
        return new List<LlmStreamEvent>
        {
            new LlmStreamMessageStart
            {
                Message = this.MapInitialEventToLlmResponse(streamEvent),
            },
            new LlmStreamContentStart
            {
                Index = streamEvent.Choices.FirstOrDefault()?.Index ?? 0,
            },
        };
    }

    public LlmStreamContentDelta? MapContentDelta(OpenAiStreamEvent streamEvent)
    {
        var choice = streamEvent.Choices.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(choice?.Delta.Content))
        {
            return default;
        }

        this.estimatedOutputTokens++;
        return new LlmStreamContentDelta
        {
            Index = choice.Index,
            Delta = new LlmTextContent
            {
                Text = choice.Delta.Content,
            },
        };
    }

    public List<LlmStreamEvent> MapClosingEvents(OpenAiStreamEvent streamEvent)
    {
        var usage = streamEvent.Usage!;
        this.inputTokens = usage.PromptTokens;
        this.outputTokens = usage.CompletionTokens;

        return new List<LlmStreamEvent>
        {
            new LlmStreamContentStop
            {
                Index = streamEvent.Choices.FirstOrDefault()?.Index ?? 0,
            },
            new LlmStreamMessageStop
            {
                StopReason = streamEvent.Choices.FirstOrDefault()?.FinnishReason ?? "halt",
            },
            this.CreateTotalUsageFromCurrentInformation(false),
        };
    }

    public LlmStreamTotalUsage CreateTotalUsageFromCurrentInformation(
        bool useInputEstimate,
        string? stopReason = default)
    {
        return new LlmStreamTotalUsage
        {
            ProviderPromptIdentifier = this.lastKnownProviderPromptIdentifier,
            InputTokens = useInputEstimate ? (long)this.EstimatePromptTokens() : this.inputTokens,
            OutputTokens = this.outputTokens == 0 ? (long)this.estimatedOutputTokens : this.outputTokens,
            StopReason = stopReason ?? "halt",
        };
    }

    private LlmResponse MapInitialEventToLlmResponse(OpenAiStreamEvent streamEvent)
    {
        return new LlmResponse(
            ProviderPromptIdentifier: streamEvent.Id,
            ModelId: model.Id.Value,
            ModelIdentifierName: model.ModelIdentifierName,
            Message: new LlmPromptMessageDto(
                IsUserMessage: false,
                Content: new List<LlmContent>()),
            Usage: new LlmUsage(InputTokens: this.inputTokens, OutputTokens: this.outputTokens),
            StopReason: default,
            DetailedModelIdentifierName: streamEvent.Model);
    }
}