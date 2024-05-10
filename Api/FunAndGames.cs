using System.Text.Json;
using Implementation.Json;
using Interface.Service;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;

namespace Api;

public static class FunAndGames
{
    public static async Task Run(
        ILargeLanguageModelService largeLanguageModelService)
    {
        var prompt = new LlmPromptDto(
            Model: new LlmPromptModelDto(
                ProviderName: "Anthropic",
                ModelIdentifier: Guid.Parse("2370891f-9593-4ba6-be41-56e47fa6083f")),
            SystemMessage: "Only respond in german",
            Messages: new List<LlmPromptMessageDto>
            {
                new LlmPromptMessageDto(
                    IsUserMessage: true,
                    Content: new List<LlmContent>
                    {
                        new LlmTextContent
                        {
                            Text = "Hello, World!",
                        },
                    }),
            });
        
        var result = await largeLanguageModelService.Prompt(prompt);
        if (result.IsError)
        {
            throw result.Error!;
        }

        var llmResponse = result.Unwrap();
        var json = JsonSerializer.Serialize(
            llmResponse,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true,
                Converters =
                {
                    new LlmContentConverter(),
                },
            });
        
        Console.WriteLine(json);
    }
}
