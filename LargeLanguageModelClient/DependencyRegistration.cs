using Microsoft.Extensions.DependencyInjection;

namespace LargeLanguageModelClient;

public static class DependencyRegistration
{
    public static IServiceCollection RegisterLargeLanguageModelClientDependencies(this IServiceCollection services, Uri baseAddress)
    {
        services.AddHttpClient<ILargeLanguageModelClient, LargeLanguageModelClient>(client =>
        {
            client.BaseAddress = baseAddress;
        });

        return services;
    }
}
