using System.Text;
using Api.Attributes;
using Domain.Configuration;
using Microsoft.Extensions.Options;

namespace Api.Middleware;

public class BasicAuthenticationHandler(
    ILogger<BasicAuthenticationHandler> logger,
    IOptions<CredentialsOptions> credentialsOptions) : IMiddleware
{
    private const string BasicHeaderStart = "Basic ";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var authAttribute = this.IsProtectedByBasicAuthorization(context);
            if (authAttribute is null)
            {
                await next(context);
                return;
            }

            var authorizationHeader = context.Request.Headers.Authorization
                .FirstOrDefault(h => h?.StartsWith(BasicHeaderStart) ?? false);
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var base64String = authorizationHeader
                .Split(BasicHeaderStart)[1];
            
            var base64EncodedBytes = Convert.FromBase64String(base64String);
            var usernamePassword = Encoding.UTF8.GetString(base64EncodedBytes).Split(':');
            var username = usernamePassword[0];
            var password = usernamePassword[1];

            foreach (var user in credentialsOptions.Value.Users)
            {
                if (user.Username != username)
                {
                    continue;
                }

                if (user.Password != password)
                {
                    continue;
                }

                if (user.Admin != authAttribute.Admin)
                {
                    continue;
                }

                await next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Error during Basic auth");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }

    private BasicAuthAttribute? IsProtectedByBasicAuthorization(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            return default;
        }

        return (BasicAuthAttribute?)endpoint.Metadata.FirstOrDefault(m => m is BasicAuthAttribute);
    }
}
