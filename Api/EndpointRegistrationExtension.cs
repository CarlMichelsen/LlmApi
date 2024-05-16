using Api.Endpoints;

namespace Api;

public static class EndpointRegistrationExtension
{
    public static void UseApplicationEndpoints(this WebApplication app)
    {
        var apiGroup = app.MapGroup("api/v1");

        apiGroup.RegisterModelEndpoints();

        apiGroup.RegisterPromptEndpoints();

        apiGroup.WithOpenApi();
    }
}
