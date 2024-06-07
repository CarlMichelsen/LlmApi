using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Domain.Abstraction;
using Domain.Dto.OpenAi;
using Domain.Dto.OpenAi.Content;
using Domain.Dto.OpenAi.Response.Stream;
using Domain.Entity;
using Domain.Model;
using Implementation.Json;
using Implementation.Json.Reader;
using Interface.Client;
using Interface.Json;
using Interface.Service;

namespace Implementation.Client;

public class OpenAiClient(
    HttpClient httpClient,
    IStreamLineReader streamLineReader,
    ILlmApiKeyService llmApiKeyService) : IOpenAiClient
{
    public const string CompletionsPath = "v1/chat/completions";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters =
        {
            new OpenAiContentConverter(),
        },
    };

    public async Task<Result<OpenAiResponse>> Prompt(
        OpenAiPrompt openAiPrompt,
        CancellationToken cancellationToken)
    {
        var apiKeyResult = await llmApiKeyService.GetApiKey(LlmProvider.OpenAi);
        if (apiKeyResult.IsError)
        {
            throw apiKeyResult.Error!;
        }

        if (openAiPrompt.Stream || openAiPrompt.StreamOptions is not null)
        {
            openAiPrompt = openAiPrompt with { Stream = false, StreamOptions = default };
        }

        await using (var apiKey = apiKeyResult.Unwrap())
        {
            try
            {
                var resResult = await this.Request(openAiPrompt, apiKey!, cancellationToken, false);
                if (resResult.IsError)
                {
                    return resResult.Error!;
                }

                var res = resResult.Unwrap();
                var deserialized = await res.Content.ReadFromJsonAsync<OpenAiResponse>(Options);
                return deserialized!;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }

    public async IAsyncEnumerable<Result<OpenAiStreamEvent>> PromptStream(
        OpenAiPrompt openAiPrompt,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var apiKeyResult = await llmApiKeyService.GetApiKey(LlmProvider.OpenAi);
        if (apiKeyResult.IsError)
        {
            throw apiKeyResult.Error!;
        }

        if (!openAiPrompt.Stream)
        {
            openAiPrompt = openAiPrompt with { Stream = true, StreamOptions = new OpenAiStreamOptions(IncludeUsage: true) };
        }
        
        var openAiStreamReader = new OpenAiStreamReader(streamLineReader);
        await using (var apiKey = apiKeyResult.Unwrap())
        {
            var responseResult = await this.Request(openAiPrompt, apiKey!, cancellationToken, true);
            if (responseResult.IsError)
            {
                yield return responseResult.Error!;
                yield break;
            }

            var httpResponseStream = responseResult
                .Unwrap().Content
                .ReadAsStream(cancellationToken);
            
            await foreach (var openAiEventResult in openAiStreamReader.Read(httpResponseStream, cancellationToken))
            {
                if (openAiEventResult.IsError)
                {
                    yield return openAiEventResult.Error!;
                    yield break;
                }

                yield return openAiEventResult.Unwrap();

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }
    }

    private async Task<Result<HttpResponseMessage>> Request(
        OpenAiPrompt openAiPrompt,
        ReservableApiKey apiKey,
        CancellationToken cancellationToken,
        bool stream = false)
    {
        try
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey.ApiKey);
            var json = JsonSerializer.Serialize(openAiPrompt, Options);
            var payload = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var msg = new HttpRequestMessage
            {
                Content = payload,
                Method = HttpMethod.Post,
                RequestUri = new Uri(httpClient.BaseAddress!, CompletionsPath),
            };
            var res = await httpClient.SendAsync(msg, stream ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, cancellationToken);
            httpClient.DefaultRequestHeaders.Authorization = default;
            res.EnsureSuccessStatusCode();

            return res;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}
