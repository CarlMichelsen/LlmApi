using System.Net;
using System.Reflection;
using Implementation.Client;
using Implementation.Json.Reader;
using Moq;
using Moq.Protected;

namespace UnitTest.LargeLanguageModelClient.Fakes;

public static class FakeAnthropicClientFactory
{
    public static AnthropicClient Create()
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected() // Moq.Protected offers access to protected members
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(GetMockResponseStream("./LargeLanguageModelClient/TestData/RealAnthropicStreamResponse.txt")),
            })
            .Verifiable();

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://www.twitch.tv"),
        };
        
        return new AnthropicClient(httpClient, new StreamLineReader(), new FakeLlmApiKeyService());
    }

    private static Stream GetMockResponseStream(string fileName)
    {
        // Get the current executing assembly's location
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        
        // Get the directory containing the assembly
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        
        // Combine the assembly directory with the file name to form the full path
        var filePath = Path.Combine(assemblyDirectory!, fileName);
        
        return File.OpenRead(filePath);
    }
}
