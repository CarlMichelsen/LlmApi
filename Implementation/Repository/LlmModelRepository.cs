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
        var exsisting = await applicationContext.ModelEntity
            .Include(m => m.Price)
            .FirstOrDefaultAsync(m => m.Id == modelEntity.Id); 
        if (exsisting is not null)
        {
            return new SafeUserFeedbackException("Model already exsists");
        }

        var res = await applicationContext.ModelEntity
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
        var entity = await applicationContext.ModelEntity
            .Include(m => m.Price)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (entity is null)
        {
            return new SafeUserFeedbackException("Model not found", NoActionWasTakenString);
        }

        applicationContext.ModelEntity.Remove(entity);

        var saveResult = await applicationContext.SaveChangesAsync();

        if (saveResult > 0)
        {
            return entity;
        }

        return new SafeUserFeedbackException("Model was not deleted", NoActionWasTakenString);
    }

    public async Task<Result<List<ModelEntity>>> GetAllModels()
    {
        var entities = await applicationContext.ModelEntity
            .Include(m => m.Price)
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
        var entity = await applicationContext.ModelEntity
            .Include(m => m.Price)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (entity is null)
        {
            return new SafeUserFeedbackException("Model not found");
        }

        applicationContext.Entry(entity).State = EntityState.Detached;
        return entity;
    }

    public async Task<Result<List<ModelEntity>>> GetModelsByProvider(LlmProvider provider)
    {
        var entities = await applicationContext.ModelEntity
            .Where(m => m.Provider == provider)
            .Include(m => m.Price)
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
                var entities = applicationContext.ModelEntity.ToList();
                if (entities.Count != 0)
                {
                    applicationContext.ModelEntity.RemoveRange(entities);
                }

                var prices = applicationContext.ModelEntity
                    .Include(m => m.Price)
                    .Select(m => m.Price)
                    .ToList();
                if (prices.Count != 0)
                {
                    applicationContext.PriceEntity.RemoveRange(prices);
                }

                applicationContext.SaveChanges();

                applicationContext.ModelEntity.AddRange(modelEntities);
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
        using (var transaction = applicationContext.Database.BeginTransaction())
        {
            try
            {
                var entity = await applicationContext.ModelEntity
                    .Include(m => m.Price)
                    .FirstOrDefaultAsync(m => m.Id == modelEntity.Id);

                if (entity is null)
                {
                    return new SafeUserFeedbackException("Model not found", NoActionWasTakenString);
                }

                applicationContext.ModelEntity.Remove(entity);
                applicationContext.PriceEntity.Remove(entity.Price);
                var deleted = await applicationContext.SaveChangesAsync();

                if (deleted == 0)
                {
                    await transaction.RollbackAsync();
                    return new SafeUserFeedbackException("No rows were affected in the database", "The remove part failed");
                }

                applicationContext.ModelEntity.Add(modelEntity);
                var affected = await applicationContext.SaveChangesAsync();

                if (affected == 0)
                {
                    await transaction.RollbackAsync();
                    return new SafeUserFeedbackException("No rows were affected in the database", "The insert part failed");
                }

                await transaction.CommitAsync();
                return modelEntity;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return e;
            }
        }
    }
}
