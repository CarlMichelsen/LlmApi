using Domain.Abstraction;
using Domain.Configuration;
using Domain.Entity;
using Domain.Exception;
using Domain.Model;
using Interface.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Implementation.Service;

public class LlmDistributedApiKeyService(
    ILogger<LlmApiKeyService> logger,
    IOptions<OpenAiOptions> openAiOptions,
    IOptions<AnthropicOptions> anthropicOptions,
    ICacheService cacheService) : ILlmApiKeyService
{
    public Task<Result<ReservableApiKey>> GetApiKey(LlmProvider llmProvider)
    {
        var random = new Random();
        return this.CreateAndReserveApiKey(llmProvider, random);
    }

    private async Task<Result<ReservableApiKey>> CreateAndReserveApiKey(LlmProvider llmProvider, Random? ran = default)
    {
        var random = ran ?? new Random();
        var nameKeyDict = this.GetProviderNameKeyPairs(llmProvider);
        var nameKeyList = nameKeyDict.Select(x => x.Key).ToList();
        
        string randomKeyName;
        string cacheKeyName;
        do
        {
            try
            {
                var index = random.Next(nameKeyList.Count - 1);
                randomKeyName = nameKeyList.ElementAt(index);
                cacheKeyName = this.GenerateApiKeyCacheName(llmProvider, randomKeyName);
                nameKeyList.Remove(randomKeyName);
            }
            catch (ArgumentOutOfRangeException e)
            {
                logger.LogCritical(e, "Ran out of available api keys");
                return new SafeUserFeedbackException($"Unable to reserve {Enum.GetName(llmProvider)!} api key.");
            }
        }
        while ((await cacheService.Get(cacheKeyName)) is not null);
        
        await cacheService.Set(cacheKeyName, randomKeyName);

        if (nameKeyDict.TryGetValue(randomKeyName, out var apiKey))
        {
            return new ReservableApiKey(randomKeyName, apiKey, async (key) =>
            {
                await cacheService.Remove(cacheKeyName);
            });
        }

        return new SafeUserFeedbackException($"Unable to reserve {Enum.GetName(llmProvider)!} api key.");
    }

    private Dictionary<string, string> GetProviderNameKeyPairs(LlmProvider llmProvider)
        => llmProvider switch
        {
            LlmProvider.OpenAi => openAiOptions.Value.NameKeyPairs,
            LlmProvider.Anthropic => anthropicOptions.Value.NameKeyPairs,
            _ => throw new NotImplementedException($"{Enum.GetName(llmProvider)!} provider is not supported in the ApiKeyService."),
        };

    private string GenerateApiKeyCacheName(LlmProvider llmProvider, string keyName)
        => $"llm-api-key-in-use-{Enum.GetName(llmProvider)!}-{keyName}";
}
