using Domain.Abstraction;
using Domain.Dto.Anthropic;
using Domain.Dto.Anthropic.Content;
using Domain.Dto.Anthropic.Response;
using Domain.Entity;
using Domain.Exception;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;
using LargeLanguageModelClient.Dto.Response;

namespace Implementation.Map.Llm.Anthropic;

public class AnthropicPromptMapper(ModelEntity modelEntity)
{
    public Result<LlmResponse> Map(AnthropicResponse anthropicResponse)
    {
        var messageResult = this.MapMessage(anthropicResponse);
        if (messageResult.IsError)
        {
            return messageResult.Error!;
        }

        var usageResult = this.Map(anthropicResponse.Usage);
        if (usageResult.IsError)
        {
            return usageResult.Error!;
        }

        return new LlmResponse(
            ProviderPromptIdentifier: anthropicResponse.Id,
            ModelId: modelEntity.Id.Value,
            ModelIdentifierName: anthropicResponse.Model,
            Message: messageResult.Unwrap(),
            Usage: usageResult.Unwrap(),
            StopReason: anthropicResponse.StopReason);
    }

    public Result<LlmContent> Map(AnthropicContent anthropicContent)
    {
        switch (anthropicContent)
        {
            case AnthropicImageContent imageContent:
                var img = this.MapImageContent(imageContent);
                return this.ExplicitResultConversion<LlmImageContent, LlmContent>(img);
            
            case AnthropicTextContent textContent:
                var txt = this.MapTextContent(textContent);
                return this.ExplicitResultConversion<LlmTextContent, LlmContent>(txt);
            
            default:
                return new MapException(
                    $"Attempted to map unsupported content type ${nameof(anthropicContent.Type)}");
        }
    }

    public Result<LlmUsage> Map(AnthropicUsage anthropicUsage)
    {
        return new LlmUsage(
            InputTokens: anthropicUsage.InputTokens,
            OutputTokens: anthropicUsage.OutputTokens);
    }

    public Result<AnthropicPrompt> Map(LlmPromptDto llmPromptDto)
    {
        if (modelEntity.Provider != LlmProvider.Anthropic)
        {
            return new MapException($"Model provider is not {nameof(LlmProvider.Anthropic)}");
        }

        if (modelEntity.Id.Value != llmPromptDto.Model.ModelIdentifier)
        {
            return new MapException("ModelIdentifier does not match modelEntity used for mapping");
        }

        var messageMapResults = llmPromptDto.Messages.Select(this.Map).ToList();
        var messages = new List<AnthropicMessage>();
        foreach (var messageResult in messageMapResults)
        {
            if (messageResult.IsError)
            {
                return messageResult.Error!;
            }

            messages.Add(messageResult.Unwrap());
        }

        return new AnthropicPrompt(
            Model: modelEntity.ModelIdentifierName,
            MaxTokens: modelEntity.MaxTokenCount,
            System: llmPromptDto.SystemMessage,
            Messages: messages);
    }

    public Result<AnthropicMessage> Map(LlmPromptMessageDto llmPromptMessageDto)
    {
        var contentMapResults = llmPromptMessageDto.Content.Select(this.Map).ToList();
        var contents = new List<AnthropicContent>();
        foreach (var contentResult in contentMapResults)
        {
            if (contentResult.IsError)
            {
                return contentResult.Error!;
            }

            contents.Add(contentResult.Unwrap());
        }

        return new AnthropicMessage(
            Role: llmPromptMessageDto.IsUserMessage ? "user" : "assistant",
            Content: contents);
    }

    public Result<AnthropicContent> Map(LlmContent llmContent)
    {
        switch (llmContent)
        {
            case LlmImageContent imageContent:
                var img = this.MapImageContent(imageContent);
                return this.ExplicitResultConversion<AnthropicImageContent, AnthropicContent>(img);
            
            case LlmTextContent textContent:
                var txt = this.MapTextContent(textContent);
                return this.ExplicitResultConversion<AnthropicTextContent, AnthropicContent>(txt);
            
            default:
                return new MapException(
                    $"Attempted to map unsupported content type ${nameof(llmContent.Type)}");
        }
    }

    public Result<LlmPromptMessageDto> MapMessage(AnthropicResponse anthropicResponse)
    {
        var contentMapResults = anthropicResponse.Content.Select(this.Map).ToList();
        var contents = new List<LlmContent>();
        foreach (var contentResult in contentMapResults)
        {
            if (contentResult.IsError)
            {
                return contentResult.Error!;
            }

            contents.Add(contentResult.Unwrap());
        }

        return new LlmPromptMessageDto(
            IsUserMessage: anthropicResponse.Role == "user",
            Content: contents);
    }

    public Result<AnthropicImageContent> MapImageContent(LlmImageContent llmImageContent)
    {
        return new AnthropicImageContent
        {
            Source = new AnthropicSource
            {
                Type = llmImageContent.Format,
                MediaType = llmImageContent.MediaType,
                Data = llmImageContent.Data,
            },
        };
    }

    public Result<LlmImageContent> MapImageContent(AnthropicImageContent anthropicImageContent)
    {
        return new LlmImageContent
        {
            MediaType = anthropicImageContent.Source.MediaType,
            Data = anthropicImageContent.Source.Data,
        };
    }

    public Result<AnthropicTextContent> MapTextContent(LlmTextContent llmTextContent)
    {
        return new AnthropicTextContent
        {
            Text = llmTextContent.Text,
        };
    }

    public Result<LlmTextContent> MapTextContent(AnthropicTextContent anthropicTextContent)
    {
        return new LlmTextContent
        {
            Text = anthropicTextContent.Text,
        };
    }

    private Result<TReturn> ExplicitResultConversion<T, TReturn>(Result<T> result)
        where T : class, TReturn
        where TReturn : class
    {
        if (result.IsError)
        {
            return result.Error!;
        }

        return new Result<TReturn>(result.Unwrap());
    }
}
