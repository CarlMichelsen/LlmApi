using Domain;
using Domain.Dto.Llm;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;

namespace Implementation.Map;

public static class LlmModelDtoMapper
{
    public static LlmModelDto Map(ModelEntity modelEntity)
    {
        var providerName = Enum.GetName(modelEntity.Provider);
        if (providerName is null)
        {
            throw new MapException("modelEntity.Provider can't be mapped to a string.");
        }

        return new LlmModelDto(
            Id: modelEntity.Id.Value,
            ProviderName: providerName,
            Price: new LlmPriceDto(
                Id: modelEntity.Price.Id.Value,
                MillionInputTokenPrice: modelEntity.Price.MillionInputTokenPrice,
                MillionOutputTokenPrice: modelEntity.Price.MillionOutputTokenPrice),
            ModelIdentifierName: modelEntity.ModelIdentifierName,
            MaxTokenCount: modelEntity.MaxTokenCount,
            ImageSupport: modelEntity.ImageSupport,
            VideoSupport: modelEntity.VideoSupport,
            JsonResponseOptimized: modelEntity.JsonResponseOptimized,
            ModelDisplayName: modelEntity.ModelDisplayName,
            ModelDescription: modelEntity.ModelDescription,
            LastUpdatedUtc: modelEntity.LastUpdatedUtc);
    }

    public static ModelEntity Map(LlmModelDto modelDto, DateTime lastUpdatedUtc)
    {
        var parsed = Enum.TryParse<LlmProvider>(modelDto.ProviderName, out var result);
        if (!parsed)
        {
            throw new MapException("modelDto.ProviderName can't be mapped to a valid LlmProvider enum.");
        }

        var modelEntityId = new ModelEntityId(modelDto.Id);
        return new ModelEntity
        {
            Id = modelEntityId,
            Provider = result,
            Price = new PriceEntity
            {
                Id = new PriceEntityId(modelDto.Price.Id),
                ModelId = modelEntityId,
                MillionInputTokenPrice = modelDto.Price.MillionInputTokenPrice,
                MillionOutputTokenPrice = modelDto.Price.MillionOutputTokenPrice,
            },
            ModelIdentifierName = modelDto.ModelIdentifierName,
            MaxTokenCount = modelDto.MaxTokenCount,
            ImageSupport = modelDto.ImageSupport,
            VideoSupport = modelDto.VideoSupport,
            JsonResponseOptimized = modelDto.JsonResponseOptimized,
            ModelDisplayName = modelDto.ModelDisplayName,
            ModelDescription = modelDto.ModelDescription,
            LastUpdatedUtc = lastUpdatedUtc,
        };
    }
}
