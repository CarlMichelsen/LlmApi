using Domain.Exception;
using LargeLanguageModelClient.Dto.Response.Stream.Event;
using Microsoft.Extensions.Logging;

namespace Implementation.Map.Llm;

public static class LlmStreamMapper
{
    public static LlmStreamError HandleSafeUserFeedback(Exception e, ILogger logger)
    {
        if (e is SafeUserFeedbackException)
        {
            var safeException = e as SafeUserFeedbackException;
            List<string> messages = [safeException!.Message, ..safeException.Details];
            return new LlmStreamError(string.Join(' ', messages));
        }

        logger.LogCritical(e, "Exception in mapper");
        return new LlmStreamError("Unhandled error occured");
    }
}
