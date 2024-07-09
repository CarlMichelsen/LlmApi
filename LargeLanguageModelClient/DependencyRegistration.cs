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
            LargeLanguageModelClientFactory.ModifyHttpClient(
                client,
                baseAddress,
                userPass.Username,
                userPass.Password);
        });

        return services;
    }
}
