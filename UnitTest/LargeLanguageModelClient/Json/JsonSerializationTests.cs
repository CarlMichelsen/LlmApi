using System.Text.Json;
using System.Text.Json.Nodes;
using LargeLanguageModelClient.Dto.Prompt.Content;

namespace UnitTest.LargeLanguageModelClient.Json;

public class JsonSerializationTests
{
    [Fact]
    public void ContentJsonSerialization()
    {
        // Arrange
        LlmContent content = new LlmTextContent
        {
            Text = "test text",
        };

        // Act
        var json = JsonSerializer.Serialize(content);
        var jsonObject = JsonNode.Parse(json)?.AsObject();

        // Assert
        Assert.NotNull(jsonObject);
        var keys = new HashSet<string>();

        foreach (var prop in jsonObject)
        {
            Assert.True(keys.Add(prop.Key), $"Duplicate key found: {prop.Key}");
        }
    }
}
