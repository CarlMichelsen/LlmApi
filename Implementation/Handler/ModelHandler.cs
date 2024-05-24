using Domain.Exception;
using Interface.Handler;
using Interface.Service;
using LargeLanguageModelClient.Dto;
using LargeLanguageModelClient.Dto.Model;

namespace Implementation.Handler;

public class ModelHandler(
    ILlmModelService llmModelService) : IModelHandler
{
    public async Task<ServiceResponse<LlmModelDto>> AddModel(LlmModelDto model)
    {
        var modelsResponse = await llmModelService.AddModel(model);
        if (modelsResponse.IsError)
        {
            return this.ErrorResponse<LlmModelDto>(modelsResponse.Error!);
        }

        var models = modelsResponse.Unwrap();
        return new ServiceResponse<LlmModelDto>(models);
    }

    public async Task<ServiceResponse<LlmModelDto>> DeleteModel(Guid id)
    {
        var modelsResponse = await llmModelService.DeleteModel(id);
        if (modelsResponse.IsError)
        {
            return this.ErrorResponse<LlmModelDto>(modelsResponse.Error!);
        }

        var models = modelsResponse.Unwrap();
        return new ServiceResponse<LlmModelDto>(models);
    }

    public async Task<ServiceResponse<List<LlmModelDto>>> GetAllModels()
    {
        var modelsResponse = await llmModelService.GetAllModels();
        if (modelsResponse.IsError)
        {
            return this.ErrorResponse<List<LlmModelDto>>(modelsResponse.Error!);
        }

        var models = modelsResponse.Unwrap();
        return new ServiceResponse<List<LlmModelDto>>(models);
    }

    public async Task<ServiceResponse<LlmModelDto>> GetModel(Guid id)
    {
        var modelResponse = await llmModelService.GetModel(id);
        if (modelResponse.IsError)
        {
            return this.ErrorResponse<LlmModelDto>(modelResponse.Error!);
        }

        var model = modelResponse.Unwrap();
        return new ServiceResponse<LlmModelDto>(model);
    }

    public async Task<ServiceResponse<List<LlmModelDto>>> GetModelsByProvider(string providerName)
    {
        var modelsResponse = await llmModelService.GetModelsByProvider(providerName);
        if (modelsResponse.IsError)
        {
            return this.ErrorResponse<List<LlmModelDto>>(modelsResponse.Error!);
        }

        var models = modelsResponse.Unwrap();
        return new ServiceResponse<List<LlmModelDto>>(models);
    }

    public async Task<ServiceResponse<List<LlmModelDto>>> SetModels(List<LlmModelDto> models)
    {
        var modelsResponse = await llmModelService.SetModels(models);
        if (modelsResponse.IsError)
        {
            return this.ErrorResponse<List<LlmModelDto>>(modelsResponse.Error!);
        }

        var setModels = modelsResponse.Unwrap();
        return new ServiceResponse<List<LlmModelDto>>(setModels);
    }

    public async Task<ServiceResponse<LlmModelDto>> UpdateModel(LlmModelDto model)
    {
        var modelsResponse = await llmModelService.UpdateModel(model);
        if (modelsResponse.IsError)
        {
            return this.ErrorResponse<LlmModelDto>(modelsResponse.Error!);
        }

        var models = modelsResponse.Unwrap();
        return new ServiceResponse<LlmModelDto>(models);
    }

    private ServiceResponse<T> ErrorResponse<T>(Exception e)
    {
        if (e is SafeUserFeedbackException)
        {
            var exception = e as SafeUserFeedbackException;
            var errors = new List<string>
            {
                exception!.Message,
            };
            
            if (exception.Details.Count > 0)
            {
                errors.AddRange(exception.Details);
            }

            return new ServiceResponse<T>(errors.ToArray());
        }

        return new ServiceResponse<T>("Internal error");
    }
}
