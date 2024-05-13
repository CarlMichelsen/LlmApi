using Domain.Configuration;
using Implementation.Client;
using Implementation.Database;
using Implementation.Handler;
using Implementation.Json.Reader;
using Implementation.Repository;
using Implementation.Service;
using Interface.Handler;
using Interface.Json;
using Interface.Repository;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api;

public static class Dependencies
{
    public static void RegisterApplicationDependencies(this WebApplicationBuilder builder)
    {
        // Database
        builder.Services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                (b) => b.MigrationsAssembly("Api"));
            
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Configuration
        builder.Configuration.AddJsonFile("secrets.json", optional: false, reloadOnChange: true);
        builder.Services
            .Configure<AnthropicOptions>(builder.Configuration.GetSection(AnthropicOptions.SectionName));
        
        // Json
        builder.Services
            .AddScoped<IStreamLineReader, StreamLineReader>();

        // Handler
        builder.Services
            .AddScoped<IModelHandler, ModelHandler>();
        
        // Repository
        builder.Services
            .AddScoped<ILlmModelRepository, LlmModelRepository>()
            .AddScoped<ILargeLanguageModelService, LargeLanguageModelService>();

        // Service
        builder.Services
            .AddScoped<ILlmModelService, LlmModelService>()
            .AddScoped<ILlmApiKeyService, LlmApiKeyService>();
        
        // Clients
        builder.Services.AddHttpClient<AnthropicClient>((sp, client) =>
        {
            var anthropicOptions = sp.GetRequiredService<IOptions<AnthropicOptions>>();
            client.BaseAddress = new Uri(anthropicOptions.Value.ApiBaseUrl);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        });

        builder.Services
            .AddScoped<GenericAnthropicClient>()
            .AddScoped<GenericLargeLanguageModelClient>();
        
        // Middleware
        builder.Services
            .AddScoped<UnhandledExceptionMiddleware>();
    }
}
