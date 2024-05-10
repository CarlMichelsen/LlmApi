using Domain.Abstraction;
using Domain.Entity;
using Domain.Entity.Id;
using Domain.Exception;
using Implementation.Database;
using Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Repository;

public class LlmModelRepository(
    ApplicationContext applicationContext) : ILlmModelRepository
{
    private const string NoActionWasTakenString = "No action was taken";

    public async Task<Result<ModelEntity>> AddModel(ModelEntity modelEntity)
    {
        var exsisting = await applicationContext.Models.FindAsync(modelEntity.Id);
        if (exsisting is not null)
        {
            return new SafeUserFeedbackException("Model already exsists");
        }

        var res = await applicationContext.Models
            .AddAsync(modelEntity);
        await applicationContext.SaveChangesAsync();

        if (res.Entity is not null)
        {
            res.State = EntityState.Detached;
            return res.Entity;
        }

        return new SafeUserFeedbackException("Model was not added");
    }

    public async Task<Result<ModelEntity>> DeleteModel(ModelEntityId id)
    {
        var entity = await applicationContext.Models.FindAsync(id);

        if (entity is null)
        {
            return new SafeUserFeedbackException("Model not found", NoActionWasTakenString);
        }

        applicationContext.Models.Remove(entity);

        var saveResult = await applicationContext.SaveChangesAsync();

        if (saveResult > 0)
        {
            return entity;
        }

        return new SafeUserFeedbackException("Model was not deleted", NoActionWasTakenString);
    }

    public async Task<Result<List<ModelEntity>>> GetAllModels()
    {
        var entities = await applicationContext.Models
            .ToListAsync();

        if (entities is null)
        {
            return new SafeUserFeedbackException("Null list when attempting to find models");
        }

        foreach (var ent in entities)
        {
            applicationContext.Entry(ent).State = EntityState.Detached;
        }

        return entities;
    }

    public async Task<Result<ModelEntity>> GetModel(ModelEntityId id)
    {
        var entity = await applicationContext.Models
            .FindAsync(id);

        if (entity is null)
        {
            return new SafeUserFeedbackException("Model not found");
        }

        applicationContext.Entry(entity).State = EntityState.Detached;

        return entity;
    }

    public async Task<Result<List<ModelEntity>>> GetModelsByProvider(LlmProvider provider)
    {
        var entities = await applicationContext.Models
            .Where(m => m.Provider == provider)
            .ToListAsync();

        if (entities is null)
        {
            return new SafeUserFeedbackException("Null list when attempting to find models");
        }

        foreach (var ent in entities)
        {
            applicationContext.Entry(ent).State = EntityState.Detached;
        }

        return entities;
    }

    public async Task<Result<List<ModelEntity>>> SetModels(List<ModelEntity> modelEntities)
    {
        using (var transaction = applicationContext.Database.BeginTransaction())
        {
            try
            {
                var entities = applicationContext.Models.ToList();
                if (entities.Count != 0)
                {
                    applicationContext.Models.RemoveRange(entities);
                }

                applicationContext.Models.AddRange(modelEntities);

                applicationContext.SaveChanges();
                await transaction.CommitAsync();

                return modelEntities;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return e;
            }
        }
    }

    public async Task<Result<ModelEntity>> UpdateModel(ModelEntity modelEntity)
    {
        var entity = await applicationContext.Models
            .FindAsync(modelEntity.Id);

        if (entity is null)
        {
            return new SafeUserFeedbackException("Model not found", NoActionWasTakenString);
        }

        applicationContext.Models.Remove(entity);
        applicationContext.Models.Add(modelEntity);

        var affected = await applicationContext.SaveChangesAsync();

        if (affected == 0)
        {
            return new SafeUserFeedbackException("No rows were affected in the database");
        }

        return modelEntity;
    }
}
