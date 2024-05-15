using Domain.Abstraction;
using Domain.Dto.OpenAi;
using Domain.Dto.OpenAi.Content;
using Domain.Dto.OpenAi.Response;
using Domain.Entity;
using Domain.Exception;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;
using LargeLanguageModelClient.Dto.Response;

namespace Implementation.Map.Llm.OpenAi;

public class OpenAiPromptMapper(
    ModelEntity modelEntity)
{
    public static async Task<Result<OpenAiContent>> Map(LlmContent llmContent)
    {
        await Task.Delay(500); // TODO: implement image support for OpenAi prompts.
        switch (llmContent)
        {
            case LlmTextContent content:
                return new OpenAiTextContent
                {
                    Text = content.Text,
                };
            
            case LlmImageContent:
                return new SafeUserFeedbackException("OpenAi image content is not supported yet. Will be available soon.");

            default:
                return new MapException("A message contained an unknown content type");
        }
    }

    public async Task<Result<OpenAiPrompt>> Map(
        LlmPromptDto llmPromptDto)
    {
        try
        {
            var messageResultTasks = llmPromptDto.Messages.Select(this.Map).ToList();
            await Task.WhenAll(messageResultTasks);

            var messages = new List<OpenAiMessage>();
            foreach (var messageResult in messageResultTasks.Select(mt => mt.Result))
            {
                if (messageResult.IsError)
                {
                    return messageResult.Error!;
                }

                messages.Add(messageResult.Unwrap());
            }

            if (!string.IsNullOrWhiteSpace(llmPromptDto.SystemMessage))
            {
                var systemMessage = new OpenAiMessage(
                    Role: "system",
                    new List<OpenAiContent>
                    {
                        new OpenAiTextContent
                        {
                            Text = llmPromptDto.SystemMessage,
                        },
                    });

                messages.Insert(0, systemMessage);
            }

            return new OpenAiPrompt(
                Model: modelEntity.ModelIdentifierName,
                Messages: messages,
                MaxTokens: modelEntity.MaxTokenCount);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Result<OpenAiMessage>> Map(
        LlmPromptMessageDto message)
    {
        var contentResultTasks = message.Content.Select(Map).ToList();
        await Task.WhenAll(contentResultTasks);

        var content = new List<OpenAiContent>();
        foreach (var contentResult in contentResultTasks.Select(mt => mt.Result))
        {
            if (contentResult.IsError)
            {
                return contentResult.Error!;
            }

            content.Add(contentResult.Unwrap());
        }

        return new OpenAiMessage(
            Role: message.IsUserMessage ? "user" : "assistant",
            Content: content);
    }

    public Result<LlmPromptMessageDto> Map(OpenAiSimpleMessage openAiSimpleMessage)
    {
        return new LlmPromptMessageDto(
            IsUserMessage: openAiSimpleMessage.Role == "user",
            Content: new List<LlmContent>
            {
                new LlmTextContent
                {
                    Text = openAiSimpleMessage.Content,
                },
            });
    }

    public Result<LlmUsage> Map(OpenAiUsage openAiUsage)
    {
        return new LlmUsage(
            InputTokens: openAiUsage.PromptTokens,
            OutputTokens: openAiUsage.CompletionTokens);
    }

    public Result<LlmResponse> Map(
        OpenAiResponse openAiResponse)
    {
        var choice = openAiResponse.Choices.First();
        var messageResult = this.Map(choice.Message);
        if (messageResult.IsError)
        {
            return messageResult.Error!;
        }

        var usageResult = this.Map(openAiResponse.Usage);
        if (usageResult.IsError)
        {
            return usageResult.Error!;
        }

        return new LlmResponse(
            ProviderPromptIdentifier: openAiResponse.Id,
            ModelId: modelEntity.Id.Value,
            ModelIdentifierName: modelEntity.ModelIdentifierName,
            Message: messageResult.Unwrap(),
            Usage: usageResult.Unwrap(),
            StopReason: choice.FinnishReason,
            DetailedModelIdentifierName: openAiResponse.Model);
    }
}
