using Domain.Entity;
using Domain.Entity.Id;
using Implementation.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Repository;

public class LlmModelRepositoryTests : DatabaseTest
{
    public LlmModelRepositoryTests()
    {
        this.SUT = new LlmModelRepository(this.Context);
    }

    protected LlmModelRepository SUT { get; init; }

    [Fact]
    public async Task AddShouldWork()
    {
        // Arrange
        var identifier = Guid.NewGuid();
        var model = this.MockModelEntity(identifier, Guid.NewGuid());

        // Act
        var result = await this.SUT.AddModel(model);
        var reFetched = await this.Context.ModelEntity.FirstOrDefaultAsync(m => m.Id == model.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(reFetched);

        var value = result.Unwrap();
        ModelEqual(model, value);
        ModelEqual(model, reFetched);
    }

    [Fact]
    public async Task SetModelsTransactionShouldRollbackOnFailure()
    {
        // Arrange
        var initialSetList = new List<Domain.Entity.ModelEntity>
        {
            this.MockModelEntity(Guid.NewGuid(), Guid.NewGuid()),
            this.MockModelEntity(Guid.NewGuid(), Guid.NewGuid()),
            this.MockModelEntity(Guid.NewGuid(), Guid.NewGuid()),
            this.MockModelEntity(Guid.NewGuid(), Guid.NewGuid()),
            this.MockModelEntity(Guid.NewGuid(), Guid.NewGuid()),
        };

        var initialSetResult = await this.SUT.SetModels(initialSetList);

        // Act
        var identifier = Guid.NewGuid();
        var invalidModels = new List<Domain.Entity.ModelEntity>
        {
            this.MockModelEntity(identifier, Guid.NewGuid()),
            this.MockModelEntity(identifier, Guid.NewGuid()),
            this.MockModelEntity(Guid.NewGuid(), Guid.NewGuid()),
            this.MockModelEntity(identifier, Guid.NewGuid()),
            this.MockModelEntity(Guid.NewGuid(), Guid.NewGuid()),
        };

        var setResult = await this.SUT.SetModels(invalidModels);

        // Assert
        Assert.False(setResult.IsSuccess);
        Assert.True(initialSetResult.IsSuccess);

        var currentList = await this.Context.ModelEntity.ToListAsync();
        foreach (var initialModel in initialSetList)
        {
            ModelEqual(initialModel, currentList.First(cl => cl.Id == initialModel.Id));
        }
    }

    [Fact]
    public async Task DeleteModelShouldWork()
    {
        // Arrange
        var identifier = Guid.NewGuid();
        var model = this.MockModelEntity(identifier, Guid.NewGuid());
        this.Context.ModelEntity.Add(model);
        await this.Context.SaveChangesAsync();

        // Act
        var result = await this.SUT.DeleteModel(model.Id);
        var reFetched = await this.Context.ModelEntity.FirstOrDefaultAsync(m => m.Id == model.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(reFetched);

        var value = result.Unwrap();
        ModelEqual(model, value);
    }

    [Fact]
    public async Task UpdateShouldWork()
    {
        // Arrange
        var identifier = Guid.NewGuid();
        var exsisting = this.MockModelEntity(identifier, Guid.NewGuid());
        var updated = this.MockModelEntity(identifier, Guid.NewGuid());
        this.Context.ModelEntity.Add(exsisting);
        await this.Context.SaveChangesAsync();

        // Act
        var result = await this.SUT.UpdateModel(updated);

        // Assert
        Assert.True(result.IsSuccess);

        var value = result.Unwrap();
        ModelEqual(updated, value);
    }

    private static void ModelEqual(ModelEntity m1, ModelEntity m2)
    {
        Assert.NotNull(m1);
        Assert.NotNull(m2);
        Assert.Equal(m2.Id, m1.Id);
        Assert.Equal(m2.Provider, m1.Provider);
        Assert.Equal(m2.ModelIdentifierName, m1.ModelIdentifierName);
        Assert.Equal(m2.MaxTokenCount, m1.MaxTokenCount);
        Assert.Equal(m2.ImageSupport, m1.ImageSupport);
        Assert.Equal(m2.VideoSupport, m1.VideoSupport);
        Assert.Equal(m2.JsonResponseOptimized, m1.JsonResponseOptimized);
        Assert.Equal(m2.ModelDisplayName, m1.ModelDisplayName);
        Assert.Equal(m2.ModelDescription, m1.ModelDescription);
        Assert.Equal(m2.LastUpdatedUtc, m1.LastUpdatedUtc);
    }

    private static TEnum RandomEnum<TEnum>(Random? ran = default)
    {
        var random = ran ?? new Random();
        var values = Enum.GetValues(typeof(TEnum));
        return (TEnum)values.GetValue(random.Next(values.Length))!;
    }

    private ModelEntity MockModelEntity(Guid identifier, Guid priceIdentifier, Random? ran = default)
    {
        var random = ran ?? new Random();
        var modelEntityId = new ModelEntityId(identifier);
        return new ModelEntity
        {
            Id = modelEntityId,
            Available = true,
            Provider = RandomEnum<LlmProvider>(random),
            Price = new PriceEntity
            {
                Id = new PriceEntityId(priceIdentifier),
                ModelId = modelEntityId,
                MillionInputTokenPrice = random.Next(50, 2000),
                MillionOutputTokenPrice = random.Next(50, 4000),
            },
            ModelIdentifierName = "TestModel",
            MaxTokenCount = random.NextInt64(4032, 8064),
            ContextTokenCount = random.NextInt64(8064, 8064 * 2),
            ImageSupport = random.Next(1) == 0,
            VideoSupport = random.Next(1) == 0,
            JsonResponseOptimized = random.Next(1) == 0,
            ModelDisplayName = "TestModelDisplayName",
            ModelDescription = "TestModel Description",
            LastUpdatedUtc = DateTime.UtcNow,
        };
    }
}
