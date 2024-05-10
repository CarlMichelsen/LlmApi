using Domain.Abstraction;
using Domain.Dto.Llm;

namespace Interface.Service;

public interface ILlmModelService
{
    Task<Result<List<LlmModelDto>>> GetAllModels();

    Task<Result<List<LlmModelDto>>> GetModelsByProvider(string provider);

    Task<Result<LlmModelDto>> GetModel(Guid id);
    
    Task<Result<LlmModelDto>> DeleteModel(Guid id);

    Task<Result<LlmModelDto>> UpdateModel(LlmModelDto modelDto);

    Task<Result<LlmModelDto>> AddModel(LlmModelDto modelDto);

    Task<Result<List<LlmModelDto>>> SetModels(List<LlmModelDto> modelEntities);
}
