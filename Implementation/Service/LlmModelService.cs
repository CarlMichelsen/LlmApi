using Domain.Abstraction;
using Domain.Entity;
using Domain.Exception;
using Implementation.Map;
using Implementation.Validator;
using Interface.Repository;
using Interface.Service;
using LargeLanguageModelClient.Dto.Model;

namespace Implementation.Service;

public class LlmModelService(
    ILlmModelRepository llmModelRepository) : ILlmModelService
{
    private LlmModelDtoValidator llmModelDtoValidator = new();

    public async Task<Result<LlmModelDto>> AddModel(LlmModelDto modelDto)
    {
        var validationResult = this.llmModelDtoValidator.Validate(modelDto);
        if (!validationResult.IsValid)
        {
            var stringValidationErrors = validationResult.Errors
                .Select(error => error.ErrorMessage)
                .ToList();
            var details = string.Join('\n', stringValidationErrors);

            return new SafeUserFeedbackException("Invalid request", details!);
        }

        var modelEntity = LlmModelDtoMapper.Map(modelDto, DateTime.UtcNow);

        var addModelResult = await llmModelRepository.AddModel(modelEntity);
        if (addModelResult.IsError)
        {
            return addModelResult.Error!;
        }

        return LlmModelDtoMapper.Map(addModelResult.Unwrap());
    }

    public async Task<Result<LlmModelDto>> DeleteModel(Guid id)
    {
        var modelResult = await llmModelRepository.DeleteModel(new Domain.Entity.Id.ModelEntityId(id));
        if (modelResult.IsError)
        {
            return modelResult.Error!;
        }

        return LlmModelDtoMapper.Map(modelResult.Unwrap());
    }

    public async Task<Result<List<LlmModelDto>>> GetAllModels()
    {
        var modelsResult = await llmModelRepository.GetAllModels();
        if (modelsResult.IsError)
        {
            return modelsResult.Error!;
        }

        var models = modelsResult.Unwrap();

        return models.Select(LlmModelDtoMapper.Map).ToList();
    }

    public async Task<Result<LlmModelDto>> GetModel(Guid id)
    {
        var modelResult = await llmModelRepository.GetModel(new Domain.Entity.Id.ModelEntityId(id));
        if (modelResult.IsError)
        {
            return modelResult.Error!;
        }

        return LlmModelDtoMapper.Map(modelResult.Unwrap());
    }

    public async Task<Result<List<LlmModelDto>>> GetModelsByProvider(string provider)
    {
        var found = Enum.TryParse<LlmProvider>(provider, out var providerEnum);
        if (!found)
        {
            return new SafeUserFeedbackException("Unable to find provider");
        }

        var modelsResult = await llmModelRepository.GetModelsByProvider(providerEnum!);
        if (modelsResult.IsError)
        {
            return modelsResult.Error!;
        }

        var models = modelsResult.Unwrap();

        return models.Select(LlmModelDtoMapper.Map).ToList();
    }

    public async Task<Result<List<LlmModelDto>>> SetModels(List<LlmModelDto> modelDtos)
    {
        var validationResults = modelDtos.Select(modelDto => this.llmModelDtoValidator.Validate(modelDto));
        var stringValidationErrors = new List<string>();
        foreach (var validationResult in validationResults)
        {
            if (!validationResult.IsValid)
            {
                var stringErrors = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .ToList();

                stringValidationErrors.AddRange(stringErrors);
            }
        }

        if (stringValidationErrors.Count > 0)
        {
            var details = string.Join('\n', stringValidationErrors);
            return new SafeUserFeedbackException("Invalid request", details!);
        }

        var now = DateTime.UtcNow;
        var entities = modelDtos
            .Select(dto => LlmModelDtoMapper.Map(dto, now))
            .ToList();

        var modelsResult = await llmModelRepository.SetModels(entities);
        if (modelsResult.IsError)
        {
            return modelsResult.Error!;
        }

        var models = modelsResult.Unwrap();

        return models.Select(LlmModelDtoMapper.Map).ToList();
    }

    public async Task<Result<LlmModelDto>> UpdateModel(LlmModelDto modelDto)
    {
        var validationResult = this.llmModelDtoValidator.Validate(modelDto);
        if (!validationResult.IsValid)
        {
            var stringValidationErrors = validationResult.Errors
                .Select(error => error.ErrorMessage)
                .ToList();
            var details = string.Join('\n', stringValidationErrors);
            
            return new SafeUserFeedbackException("Invalid request", details!);
        }

        var entity = LlmModelDtoMapper.Map(modelDto, DateTime.UtcNow);
        var modelResult = await llmModelRepository.UpdateModel(entity);
        if (modelResult.IsError)
        {
            return modelResult.Error!;
        }

        return LlmModelDtoMapper.Map(modelResult.Unwrap());
    }
}
