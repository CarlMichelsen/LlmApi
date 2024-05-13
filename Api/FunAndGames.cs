using System.Text;
using Interface.Service;
using LargeLanguageModelClient;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;
using LargeLanguageModelClient.Dto.Response.Stream;
using LargeLanguageModelClient.Dto.Response.Stream.Event;

namespace Api;

public static class FunAndGames
{
    private const string Story = @"Once upon a time, in the vast realm of Linguistica"", there lived a homonym named ""Tear."" Tear was unique, for he had two distinct lives. In one life, he was a droplet of sorrow, streaming down the cheeks of those who were overwhelmed by emotions. In this form, he was soft, gentle, and a symbol of pure vulnerability. Yet, in his other life, he was a rift, a division, a force so potent he could separate the inseparable, causing fabric, paper, and even relationships to split apart. Tear's dual existence brought him much confusion among the inhabitants of Linguistica, as they would often summon him with expectations of one form, only to be confronted with the other. The Paper People of Parchment Province feared him for they loathed the tear that could divide them into fragments. Yet, the denizens of Emotion Enclave yearned for his visit, as the tear of sorrow could cleanse the soul. Tear was distressed, feeling misunderstood and misplaced in a world that couldn’t reconcile his two natures. Seeking wisdom, Tear embarked on a journey to consult the Great Owl of Orthography, a wise creature known for solving the conundrums of language and meaning. The Great Owl, with eyes wide and knowing, listened to Tear's dilemma. After a moment of thoughtful silence, the owl spoke in riddles, as it was his nature, ""Two paths you have, yet one you seek. The answer, not in division, but in union, lies. To be whole, embrace your duality, not as a curse, but as a gift to the world of Linguistica. Baffled yet enlightened, Tear took the owl's advice to heart. He began to explore how his dual nature could be a source of strength rather than a cause of confusion. Over time, with patience and perseverance, he became a symbol of transformation and resilience, teaching the inhabitants of Linguistica the value of embracing complexity and ambiguity. Tear was no longer the homonym with an identity crisis, but the emblem of depth and diversity in the fabric of meaning. And so, Tear's story became a legend in Linguistica, a tale about the beauty of being multifaceted in a world that often seeks simplicity. Though difficult for some to fully grasp, Tear's journey reminded everyone that sometimes, the essence of being cannot be neatly categorized or defined by language alone.";

    public static async Task Run(IServiceScope scope)
    {
        var largeLanguageModelService = scope.ServiceProvider.GetRequiredService<ILargeLanguageModelService>();

        var prompt = new LlmPromptDto(
            Model: new LlmPromptModelDto(
                ProviderName: "Anthropic",
                ModelIdentifier: Guid.Parse("2370891f-9593-4ba6-be41-56e47fa6083f")),
            SystemMessage: "Only respond with accurate german translations of what has been said",
            Messages: new List<LlmPromptMessageDto>
            {
                new LlmPromptMessageDto(
                    IsUserMessage: true,
                    Content: new List<LlmContent>
                    {
                        new LlmTextContent
                        {
                            Text = Story,
                        },
                    }),
            });
        
        var sb = new StringBuilder();
        Console.Clear();

        await foreach (var streamEvent in largeLanguageModelService.PromptStream(prompt))
        {
            if (streamEvent.Type == LlmStreamEventType.ContentDelta)
            {
                var contentEvent = streamEvent as LlmStreamContentDelta;
                if (contentEvent!.Delta.Type == LlmContentType.Text)
                {
                    var textContent = contentEvent.Delta as LlmTextContent;
                    sb.Append(textContent!.Text);
                }
            }
        }

        Console.WriteLine(sb.ToString());
    }
}
