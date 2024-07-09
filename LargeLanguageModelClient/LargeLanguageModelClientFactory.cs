using System.Net.Http.Headers;

namespace LargeLanguageModelClient;

public static class LargeLanguageModelClientFactory
{
    public static ILargeLanguageModelClient Create(Uri baseAddress, string username, string password)
    {
        var client = new HttpClient();
        ModifyHttpClient(client, baseAddress, username, password);
        return new LargeLanguageModelClient(client);
    }

    internal static void ModifyHttpClient(HttpClient client, Uri baseAddress, string username, string password)
    {
        client.BaseAddress = baseAddress;
        var userPassBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(userPassBytes));
    }
}
