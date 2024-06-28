using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using LargeLanguageModelClient.Dto;
using LargeLanguageModelClient.Dto.Model;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Response;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;

namespace LargeLanguageModelClient;

internal class LargeLanguageModelClient(
    HttpClient httpClient) : ILargeLanguageModelClient
{
    private const string PromptPath = "api/v1/prompt";
    private const string StreamPromptPath = "api/v1/prompt/stream";
    private const string StreamEventDelimeter = "event: ";
    private const string StreamDataDelimeter = "data: ";

    private readonly JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<ServiceResponse<List<LlmModelDto>>> GetAllModels()
    {
        var res = await httpClient.GetAsync("api/v1/model/all");
        res.EnsureSuccessStatusCode();

        var response = await res.Content.ReadFromJsonAsync<ServiceResponse<List<LlmModelDto>>>();
        return response ?? new ServiceResponse<List<LlmModelDto>>("No response");
    }

    public async Task<ServiceResponse<LlmModelDto>> GetModel(Guid modelId)
    {
        var res = await httpClient.GetAsync($"api/v1/model/{modelId}");
        res.EnsureSuccessStatusCode();

        var response = await res.Content.ReadFromJsonAsync<ServiceResponse<LlmModelDto>>();
        return response ?? new ServiceResponse<LlmModelDto>("No response");
    }

    public async Task<ServiceResponse<LlmResponse>> Prompt(LlmPromptDto llmPromptDto, CancellationToken cancellationToken)
    {
        var res = await this.Request(llmPromptDto, cancellationToken, false);
        var serviceResponse = await res.Content.ReadFromJsonAsync<ServiceResponse<LlmResponse>>(this.options);
        return serviceResponse ?? new ServiceResponse<LlmResponse>("No response");
    }

    public async IAsyncEnumerable<LlmStreamEvent> PromptStream(LlmPromptDto llmPromptDto, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        LlmStreamEventType? lastEventType = default;
        var res = await this.Request(llmPromptDto, cancellationToken, true);
        
        using var sr = new StreamReader(await res.Content.ReadAsStreamAsync(cancellationToken), Encoding.UTF8);
        while (!sr.EndOfStream)
        {
            var line = await sr.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith(StreamEventDelimeter) && lastEventType is null)
            {
                var eventInt = Convert.ToInt32(line.Substring(StreamEventDelimeter.Length).Trim());
                lastEventType = (LlmStreamEventType)eventInt;
                continue;
            }

            if (line.StartsWith(StreamDataDelimeter) && lastEventType is not null)
            {
                var data = line.Substring(StreamDataDelimeter.Length).Trim();
                yield return lastEventType switch
                {
                    LlmStreamEventType.MessageStart => JsonSerializer.Deserialize<LlmStreamMessageStart>(data, this.options)!,
                    LlmStreamEventType.ContentStart => JsonSerializer.Deserialize<LlmStreamContentStart>(data, this.options)!,
                    LlmStreamEventType.ContentDelta => JsonSerializer.Deserialize<LlmStreamContentDelta>(data, this.options)!,
                    LlmStreamEventType.ContentStop => JsonSerializer.Deserialize<LlmStreamContentStop>(data, this.options)!,
                    LlmStreamEventType.MessageStop => JsonSerializer.Deserialize<LlmStreamMessageStop>(data, this.options)!,
                    LlmStreamEventType.TotalUsage => JsonSerializer.Deserialize<LlmStreamTotalUsage>(data, this.options)!,
                    LlmStreamEventType.Error => JsonSerializer.Deserialize<LlmStreamError>(data, this.options)!,
                    _ => new LlmStreamError($"Unknown stream event type in client {(int)lastEventType}"),
                };

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                lastEventType = default;
            }
        }
    }

    private async Task<HttpResponseMessage> Request(
        LlmPromptDto prompt,
        CancellationToken cancellationToken,
        bool stream)
    {
        var json = JsonSerializer.Serialize(prompt, this.options);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        var msg = new HttpRequestMessage
        {
            Content = payload,
            Method = HttpMethod.Post,
            RequestUri = new Uri(httpClient.BaseAddress!, stream ? StreamPromptPath : PromptPath),
        };
        var res = await httpClient.SendAsync(msg, stream ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, cancellationToken);
        res.EnsureSuccessStatusCode();

        return res;
    }
}