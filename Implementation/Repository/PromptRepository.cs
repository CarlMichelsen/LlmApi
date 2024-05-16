using Domain.Entity;
using Implementation.Database;
using Interface.Repository;
using LargeLanguageModelClient;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;
using LargeLanguageModelClient.Dto.Response;
using Microsoft.Extensions.Logging;

namespace Implementation.Repository;

public class PromptRepository(
    ILogger<PromptRepository> logger,
    ApplicationContext applicationContext) : IPromptRepository
{
    public async Task TrackPrompt(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        Guid accessTokenIdentifier,
        string? providerPromptIdentifier,
        string? detailedModelIdentifier,
        TimeSpan promptCompletionTime,
        bool streamed,
        List<(LlmContentType ContentType, string Content)> llmContent,
        LlmUsage llmUsage)
    {
        foreach (var item in llmContent)
        {
            logger.LogWarning("{Model}\n{ContentType}:\n{Content}", modelEntity.ModelDisplayName, Enum.GetName(item.ContentType), item.Content);
        }

        var promptEntity = this.Map(
            llmPromptDto,
            modelEntity,
            Guid.Parse("0e151a02-1493-4e8b-8231-a84daa4e7c12"), // TODO: replace this with an actual token when access control works
            providerPromptIdentifier,
            detailedModelIdentifier,
            promptCompletionTime,
            streamed,
            llmContent,
            llmUsage);
        
        applicationContext.PromptEntity.Add(promptEntity);
        await applicationContext.SaveChangesAsync();
    }

    private PromptEntity Map(
        LlmPromptDto llmPromptDto,
        ModelEntity modelEntity,
        Guid accessTokenIdentifier,
        string? providerPromptIdentifier,
        string? detailedModelIdentifier,
        TimeSpan promptCompletionTime,
        bool streamed,
        List<(LlmContentType ContentType, string Content)> llmContent,
        LlmUsage llmUsage)
    {
        return new PromptEntity
        {
            AccessTokenIdentifier = accessTokenIdentifier,
            InternalModelIdentifier = modelEntity.Id.Value,
            ProviderPromptIdentifier = providerPromptIdentifier,
            Model = detailedModelIdentifier ?? modelEntity.ModelIdentifierName,
            Streamed = streamed,
            PromptCompletionTime = promptCompletionTime,
            CurrentMillionInputTokenPrice = modelEntity.Price.MillionInputTokenPrice,
            CurrentMillionOutputTokenPrice = modelEntity.Price.MillionOutputTokenPrice,
            InputTokens = llmUsage.InputTokens,
            OutputTokens = llmUsage.OutputTokens,
            SystemMessage = llmPromptDto.SystemMessage,
            Messages = llmPromptDto.Messages.Select(this.Map).ToList(),
            ResponseMessage = new PromptMessageEntity
            {
                IsUserMessage = false,
                Content = llmContent.Select(x => new PromptContentEntity
                {
                    ContentType = Enum.GetName(x.ContentType)!.ToLower(),
                    Content = x.Content,
                }).ToList(),
            },
            PromptFinnishedUtc = DateTime.UtcNow,
        };
    }

    private PromptMessageEntity Map(LlmPromptMessageDto llmPromptMessageDto)
    {
        return new PromptMessageEntity
        {
            IsUserMessage = llmPromptMessageDto.IsUserMessage,
            Content = llmPromptMessageDto.Content.Select(this.Map).ToList(),
        };
    }

    private PromptContentEntity Map(LlmContent promptContentEntity)
    {
        return new PromptContentEntity
        {
            ContentType = Enum.GetName(promptContentEntity.Type)!.ToLower(),
            Content = promptContentEntity.GetContent(),
        };
    }
}
