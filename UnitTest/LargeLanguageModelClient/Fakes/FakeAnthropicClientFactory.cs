using Implementation.Client;
using Implementation.Json.Reader;

namespace UnitTest.LargeLanguageModelClient.Fakes;

public static class FakeAnthropicClientFactory
{
    public static AnthropicClient Create(string testFileName)
    {
        var httpClient = FakeStreamHttpClientFactory.Create(testFileName);
        return new AnthropicClient(httpClient, new StreamLineReader(), new FakeLlmApiKeyService());
    }
}
