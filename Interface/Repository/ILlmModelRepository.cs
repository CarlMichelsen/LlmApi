using Domain.Abstraction;
using Domain.Entity;
using Domain.Entity.Id;

namespace Interface.Repository;

public interface ILlmModelRepository
{
    Task<Result<List<ModelEntity>>> GetAllModels();

    Task<Result<List<ModelEntity>>> GetModelsByProvider(LlmProvider provider);

    Task<Result<ModelEntity>> GetModel(ModelEntityId id);
    
    Task<Result<ModelEntity>> DeleteModel(ModelEntityId id);

    Task<Result<ModelEntity>> UpdateModel(ModelEntity modelEntity);

    Task<Result<ModelEntity>> AddModel(ModelEntity modelEntity);

    Task<Result<List<ModelEntity>>> SetModels(List<ModelEntity> modelEntities);
}
