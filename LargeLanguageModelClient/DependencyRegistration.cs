using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace LargeLanguageModelClient;

public static class DependencyRegistration
{
    public static IServiceCollection RegisterLargeLanguageModelClientDependencies(
        this IServiceCollection services,
        Uri baseAddress,
        (string Username, string Password) userPass)
    {
        services.AddHttpClient<ILargeLanguageModelClient, LargeLanguageModelClient>(client =>
        {
            client.BaseAddress = baseAddress;
            var userPassBytes = System.Text.Encoding.UTF8.GetBytes($"{userPass.Username}:{userPass.Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(userPassBytes));
        });

        return services;
    }
}
