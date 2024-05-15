using Domain.Dto.Llm;
using Interface.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class ModelEndpoints
{
    public static WebApplication RegisterModelEndpoints(this WebApplication app, RouteGroupBuilder group)
    {
        var modelGroup = group
            .MapGroup("model")
            .WithDisplayName("Model");

        modelGroup
            .MapGet("/all", (IModelHandler modelHandler) => modelHandler.GetAllModels())
            .WithName("GetAllModels");
        
        modelGroup
            .MapGet("/provider/{provider}", ([FromRoute] string provider, IModelHandler modelHandler) => modelHandler.GetModelsByProvider(provider))
            .WithName("GetProviderModels");
        
        modelGroup
            .MapGet("/{id}", ([FromRoute] Guid id, IModelHandler modelHandler) => modelHandler.GetModel(id))
            .WithName("GetModel");
        
        modelGroup
            .MapDelete("/{id}", ([FromRoute] Guid id, IModelHandler modelHandler) => modelHandler.DeleteModel(id))
            .WithName("DeleteModel");
        
        modelGroup
            .MapPost("/", ([FromBody] LlmModelDto model, IModelHandler modelHandler) => modelHandler.AddModel(model))
            .WithName("AddModel");
        
        modelGroup
            .MapPut("/", ([FromBody] LlmModelDto model, IModelHandler modelHandler) => modelHandler.UpdateModel(model))
            .WithName("UpdateModel");
        
        modelGroup
            .MapPost("/forcesetmodels", ([FromBody] List<LlmModelDto> models, IModelHandler modelHandler) => modelHandler.SetModels(models))
            .WithName("ForceSetModels");

        return app;
    }
}