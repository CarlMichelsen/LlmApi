using Implementation.Client;
using Implementation.Json.Reader;

namespace UnitTest.LargeLanguageModelClient.Fakes;

public static class FakeOpenAiClientFactory
{
    public static OpenAiClient Create(string testFileName)
    {
        var httpClient = FakeStreamHttpClientFactory.Create(testFileName);
        return new OpenAiClient(httpClient, new StreamLineReader(), new FakeLlmApiKeyService());
    }
}
