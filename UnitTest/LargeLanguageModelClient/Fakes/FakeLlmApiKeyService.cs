using Domain.Abstraction;
using Domain.Entity;
using Domain.Model;
using Interface.Service;

namespace UnitTest.LargeLanguageModelClient.Fakes;

public class FakeLlmApiKeyService : ILlmApiKeyService
{
    public async Task<Result<ReservableApiKey>> GetApiKey(LlmProvider llmProvider)
    {
        await Task.Delay(100);
        return new ReservableApiKey(
            "mock-test-key",
            "fake-api-key",
            (_) => Task.CompletedTask);
    }
}
