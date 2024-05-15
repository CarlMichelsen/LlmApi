using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Domain.Abstraction;
using Domain.Dto.Anthropic;
using Domain.Dto.Anthropic.Response.Stream;
using Domain.Model;
using Implementation.Json;
using Implementation.Json.Reader;
using Interface.Client;
using Interface.Json;
using Interface.Service;

namespace Implementation.Client;

public class AnthropicClient(
    HttpClient httpClient,
    IStreamLineReader streamLineReader,
    ILlmApiKeyService llmApiKeyService) : IAnthropicClient
{
    private const string ApiKeyHeaderName = "x-api-key";
    private const string MessagesApi = "v1/messages";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters =
        {
            new AnthropicContentConverter(),
        },
    };

    public async Task<Result<AnthropicResponse>> Prompt(
        AnthropicPrompt anthropicPrompt,
        CancellationToken cancellationToken)
    {
        var apiKeyResult = await llmApiKeyService.GetApiKey(Domain.Entity.LlmProvider.Anthropic);
        if (apiKeyResult.IsError)
        {
            throw apiKeyResult.Error!;
        }

        if (anthropicPrompt.Stream)
        {
            anthropicPrompt = anthropicPrompt with { Stream = false };
        }

        await using (var apiKey = apiKeyResult.Unwrap())
        {
            try
            {
                var resResult = await this.Request(anthropicPrompt, apiKey!, cancellationToken, false);
                if (resResult.IsError)
                {
                    return resResult.Error!;
                }

                var res = resResult.Unwrap();
                var deserialized = await res.Content.ReadFromJsonAsync<AnthropicResponse>(Options);
                return deserialized!;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }

    public async IAsyncEnumerable<Result<AnthropicStreamEvent>> PromptStream(
        AnthropicPrompt anthropicPrompt,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var apiKeyResult = await llmApiKeyService.GetApiKey(Domain.Entity.LlmProvider.Anthropic);
        if (apiKeyResult.IsError)
        {
            throw apiKeyResult.Error!;
        }

        if (!anthropicPrompt.Stream)
        {
            anthropicPrompt = anthropicPrompt with { Stream = true };
        }

        var anthropicStreamReader = new AnthropicStreamReader(streamLineReader);
        await using (var apiKey = apiKeyResult.Unwrap())
        {
            var responseResult = await this.Request(anthropicPrompt, apiKey!, cancellationToken, true);
            if (responseResult.IsError)
            {
                yield return responseResult.Error!;
                yield break;
            }

            var httpResponseStream = responseResult
                .Unwrap().Content
                .ReadAsStream(cancellationToken);
            
            await foreach (var anthropicEventResult in anthropicStreamReader.Read(httpResponseStream, cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                if (anthropicEventResult.IsError)
                {
                    yield return anthropicEventResult.Error!;
                    yield break;
                }

                var anthropicEvent = anthropicEventResult.Unwrap();
                yield return anthropicEvent;
            }
        }
    }

    private async Task<Result<HttpResponseMessage>> Request(
        AnthropicPrompt anthropicPrompt,
        ReservableApiKey apiKey,
        CancellationToken cancellationToken,
        bool stream = false)
    {
        try
        {
            httpClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, apiKey.ApiKey);
            var json = JsonSerializer.Serialize(anthropicPrompt, Options);
            var payload = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var msg = new HttpRequestMessage
            {
                Content = payload,
                Method = HttpMethod.Post,
                RequestUri = new Uri(httpClient.BaseAddress!, MessagesApi),
            };
            var res = await httpClient.SendAsync(msg, stream ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, cancellationToken);
            httpClient.DefaultRequestHeaders.Remove(ApiKeyHeaderName);
            res.EnsureSuccessStatusCode();

            return res;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}
