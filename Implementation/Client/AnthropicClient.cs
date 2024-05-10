using System.Net.Http.Json;
using System.Text.Json;
using Domain.Dto.Anthropic;
using Domain.Model;
using Implementation.Json;
using Interface.Client;
using Interface.Service;
using Microsoft.Extensions.Logging;

namespace Implementation.Client;

public class AnthropicClient(
    HttpClient httpClient,
    ILlmApiKeyService llmApiKeyService) : IAnthropicClient
{
    private const string ApiKeyHeaderName = "x-api-key";
    private const string MessagesApi = "v1/messages";

    private static JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters =
        {
            new AnthropicContentConverter(),
        },
    };

    public async Task<AnthropicResponse> Prompt(AnthropicPrompt anthropicPrompt)
    {
        var apiKeyResult = await llmApiKeyService.GetApiKey(Domain.Entity.LlmProvider.Anthropic);
        if (apiKeyResult.IsError)
        {
            throw apiKeyResult.Error!;
        }

        await using (var apiKey = apiKeyResult.Unwrap())
        {
            var res = await this.Request(anthropicPrompt, apiKey!);
            res.EnsureSuccessStatusCode();

            var deserialized = await res.Content.ReadFromJsonAsync<AnthropicResponse>(options);
            var json = JsonSerializer.Serialize(deserialized, options);
            return deserialized!;
        }
    }

    private Task<HttpResponseMessage> Request(AnthropicPrompt anthropicPrompt, ReservableApiKey apiKey)
    {
        httpClient.DefaultRequestHeaders.Remove(ApiKeyHeaderName);
        httpClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, apiKey.ApiKey);
        var json = JsonSerializer.Serialize(anthropicPrompt, options);
        var payload = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return httpClient.PostAsync(MessagesApi, payload);
    }
}
