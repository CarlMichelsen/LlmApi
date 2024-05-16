using Api.Attributes;
using Interface.Handler;
using LargeLanguageModelClient.Dto.Prompt;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class PromptEndpoints
{
    public static RouteGroupBuilder RegisterPromptEndpoints(this RouteGroupBuilder group)
    {
        var modelGroup = group
            .MapGroup("prompt")
            .WithMetadata(new BasicAuthAttribute(false))
            .WithTags("Prompt");
        
        modelGroup
            .MapPost(
                "/",
                async ([FromServices] IPromptHandler promptHandler, [FromBody] LlmPromptDto llmPromptDto, CancellationToken cancellationToken) =>
                    await promptHandler.Prompt(llmPromptDto, cancellationToken))
            .WithName("Prompt");
        
        modelGroup
            .MapPost(
                "/stream",
                async ([FromServices] IStreamPromptHandler promptStreamHandler, [FromBody] LlmPromptDto llmPromptDto, CancellationToken cancellationToken) =>
                    await promptStreamHandler.StreamPrompt(llmPromptDto, cancellationToken))
            .WithName("PromptStream");

        return modelGroup;
    }
}
