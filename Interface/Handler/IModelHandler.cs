using LargeLanguageModelClient.Dto;
using LargeLanguageModelClient.Dto.Model;

namespace Interface.Handler;

public interface IModelHandler
{
    Task<ServiceResponse<List<LlmModelDto>>> GetAllModels();
    
    Task<ServiceResponse<List<LlmModelDto>>> GetModelsByProvider(string providerName);

    Task<ServiceResponse<LlmModelDto>> GetModel(Guid id);

    Task<ServiceResponse<LlmModelDto>> DeleteModel(Guid id);

    Task<ServiceResponse<LlmModelDto>> UpdateModel(LlmModelDto model);

    Task<ServiceResponse<LlmModelDto>> AddModel(LlmModelDto model);

    Task<ServiceResponse<List<LlmModelDto>>> SetModels(List<LlmModelDto> models);
}
