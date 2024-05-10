using Domain.Abstraction;
using Domain.Entity;
using Domain.Model;

namespace Interface.Service;

public interface ILlmApiKeyService
{
    Task<Result<ReservableApiKey>> GetApiKey(LlmProvider llmProvider);
}
