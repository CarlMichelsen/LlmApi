using Api.Attributes;
using Interface.Handler;
using LargeLanguageModelClient.Dto.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class ModelEndpoints
{
    private const string ModelTag = "Model";

    private const string AdminTag = "Model Administration";

    private static readonly BasicAuthAttribute AdminAuthAttribute = new BasicAuthAttribute(true);

    private static readonly BasicAuthAttribute UserAuthAttribute = new BasicAuthAttribute(false);

    public static RouteGroupBuilder RegisterModelEndpoints(this RouteGroupBuilder group)
    {
        var modelGroup = group
            .MapGroup("model");

        modelGroup
            .MapGet("/all", (IModelHandler modelHandler) => modelHandler.GetAllModels())
            .WithTags(ModelTag)
            .WithMetadata(UserAuthAttribute)
            .WithName("GetAllModels");
        
        modelGroup
            .MapGet("/provider/{provider}", ([FromRoute] string provider, IModelHandler modelHandler) => modelHandler.GetModelsByProvider(provider))
            .WithTags(ModelTag)
            .WithMetadata(UserAuthAttribute)
            .WithName("GetProviderModels");
        
        modelGroup
            .MapGet("/{id}", ([FromRoute] Guid id, IModelHandler modelHandler) => modelHandler.GetModel(id))
            .WithTags(ModelTag)
            .WithMetadata(UserAuthAttribute)
            .WithName("GetModel");
        
        modelGroup
            .MapDelete("/{id}", ([FromRoute] Guid id, IModelHandler modelHandler) => modelHandler.DeleteModel(id))
            .WithTags(AdminTag)
            .WithMetadata(AdminAuthAttribute)
            .WithName("DeleteModel");
        
        modelGroup
            .MapPost("/", ([FromBody] LlmModelDto model, IModelHandler modelHandler) => modelHandler.AddModel(model))
            .WithTags(AdminTag)
            .WithMetadata(AdminAuthAttribute)
            .WithName("AddModel");
        
        modelGroup
            .MapPut("/", ([FromBody] LlmModelDto model, IModelHandler modelHandler) => modelHandler.UpdateModel(model))
            .WithTags(AdminTag)
            .WithMetadata(AdminAuthAttribute)
            .WithName("UpdateModel");
        
        modelGroup
            .MapPost("/forcesetmodels", ([FromBody] List<LlmModelDto> models, IModelHandler modelHandler) => modelHandler.SetModels(models))
            .WithTags(AdminTag)
            .WithMetadata(AdminAuthAttribute)
            .WithName("ForceSetModels");

        return modelGroup;
    }
}