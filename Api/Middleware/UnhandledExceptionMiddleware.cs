namespace Api;

public sealed class UnhandledExceptionMiddleware(
    ILogger<UnhandledExceptionMiddleware> logger) : IMiddleware
{
    private const string TraceIdHeaderName = "X-Trace-Id";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            this.EnsureTraceIdIsApplied(context);
            await next(context);
        }
        catch (Exception e)
        {
            var traceId = this.GetTraceId(context);
            logger.LogCritical(
                e,
                "Unhandled exception TraceId: {TraceId}",
                traceId);
            
            context.Response.StatusCode = 500;
        }
    }

    private void EnsureTraceIdIsApplied(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey(TraceIdHeaderName))
        {
            context.Response.Headers[TraceIdHeaderName] = context.Request.Headers[TraceIdHeaderName];
        }
        else
        {
            var traceId = Guid.NewGuid().ToString();
            context.Response.Headers[TraceIdHeaderName] = traceId;
            logger.LogDebug("Generated Trace ID: {TraceId}", traceId);
        }
    }

    private string? GetTraceId(HttpContext context)
    {
        if (context.Response.Headers.TryGetValue(TraceIdHeaderName, out var traceIdValues))
        {
            return traceIdValues.FirstOrDefault();
        }

        return default;
    }
}