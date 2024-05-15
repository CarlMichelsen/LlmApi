using Domain.Abstraction;
using Domain.Configuration;
using Domain.Entity;
using Domain.Exception;
using Domain.Model;
using Interface.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Implementation.Service;

public class LlmApiKeyService(
    ILogger<LlmApiKeyService> logger,
    IOptions<OpenAiOptions> openAiOptions,
    IOptions<AnthropicOptions> anthropicOptions) : ILlmApiKeyService
{
    private static Dictionary<LlmProvider, List<string>> keysInUse = new();

    public async Task<Result<ReservableApiKey>> GetApiKey(LlmProvider llmProvider)
    {
        var nameKeyPairResult = await this.GetAvailableNameKeyPair(llmProvider);
        if (nameKeyPairResult.IsError)
        {
            return nameKeyPairResult.Error!;
        }

        var nameKeyPair = nameKeyPairResult.Unwrap();
        return new ReservableApiKey(
            name: nameKeyPair.Key,
            apiKey: nameKeyPair.Value,
            unlockAction: (ReservableApiKey key) =>
            {
                if (keysInUse.TryGetValue(llmProvider, out var reservedKeysForProvider))
                {
                    reservedKeysForProvider.Remove(key.Name);
                }

                return Task.CompletedTask;
            });
    }

    private Task<Result<KeyValuePair<string, string>>> GetAvailableNameKeyPair(LlmProvider llmProvider, Random? ran = default)
    {
        var found = keysInUse.TryGetValue(llmProvider, out var reservedKeysForProvider);
        if (!found)
        {
            var list = new List<string>();
            keysInUse.Add(llmProvider, list);
            reservedKeysForProvider = list;
        }

        var keyNameDictionaryResult = this.GetNameKeysForProvider(llmProvider);
        if (keyNameDictionaryResult.IsError)
        {
            return Task.FromResult<Result<KeyValuePair<string, string>>>(keyNameDictionaryResult.Error!);
        }

        var keyNameDictionary = keyNameDictionaryResult.Unwrap();

        var availableApiKeyNames = keyNameDictionary
            .Select(kv => kv.Key)
            .Where(name => !reservedKeysForProvider!.Contains(name))
            .ToList();
        
        if (availableApiKeyNames.Count == 0)
        {
            var exception = new NoAvailableApiKeyException("No available api keys left", llmProvider);
            logger.LogCritical(exception, "Ran out of api-keys");
            return Task.FromResult<Result<KeyValuePair<string, string>>>(exception);
        }
        
        var random = ran ?? new Random();
        var randomAvailableApiKeyName = availableApiKeyNames[random.Next(availableApiKeyNames.Count)];
        if (keyNameDictionary.TryGetValue(randomAvailableApiKeyName, out var availableApiKey))
        {
            var pair = new KeyValuePair<string, string>(randomAvailableApiKeyName, availableApiKey);
            return Task.FromResult<Result<KeyValuePair<string, string>>>(pair);
        }

        return Task.FromResult<Result<KeyValuePair<string, string>>>(new NoAvailableApiKeyException("Failed to find api key by name?", llmProvider));
    }

    private Result<Dictionary<string, string>> GetNameKeysForProvider(LlmProvider llmProvider)
        => llmProvider switch
        {
            LlmProvider.OpenAi => openAiOptions.Value.NameKeyPairs,
            LlmProvider.Anthropic => anthropicOptions.Value.NameKeyPairs,
            _ => throw new NotImplementedException($"{Enum.GetName(llmProvider)} is not implemented in LlmApiKeyService"),
        };
}
