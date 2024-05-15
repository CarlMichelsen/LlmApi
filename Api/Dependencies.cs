using Domain.Configuration;
using Implementation.Client;
using Implementation.Database;
using Implementation.Handler;
using Implementation.Json.Reader;
using Implementation.Repository;
using Implementation.Service;
using Interface.Client;
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
            .Configure<AdminOptions>(builder.Configuration.GetSection(AdminOptions.SectionName))
            .Configure<OpenAiOptions>(builder.Configuration.GetSection(OpenAiOptions.SectionName))
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
            .AddScoped<IPromptRepository, PromptRepository>();

        // Service
        builder.Services
            .AddScoped<ILlmModelService, LlmModelService>()
            .AddScoped<ILlmApiKeyService, LlmApiKeyService>()
            .AddScoped<LargeLanguageModelService>()
            .AddScoped<ILargeLanguageModelService, TrackedLargeLanguageModelService>();
        
        // Clients
        builder.Services.AddHttpClient<IAnthropicClient, AnthropicClient>((sp, client) =>
        {
            var anthropicOptions = sp.GetRequiredService<IOptions<AnthropicOptions>>();
            client.BaseAddress = new Uri(anthropicOptions.Value.ApiBaseUrl);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        });

        builder.Services.AddHttpClient<IOpenAiClient, OpenAiClient>((sp, client) =>
        {
            var openAiOptions = sp.GetRequiredService<IOptions<OpenAiOptions>>();
            client.BaseAddress = new Uri(openAiOptions.Value.ApiBaseUrl);
        });

        builder.Services
            .AddScoped<GenericAnthropicClient>()
            .AddScoped<GenericOpenAiClient>()
            .AddScoped<GenericLargeLanguageModelClient>();
        
        // Middleware
        builder.Services
            .AddScoped<UnhandledExceptionMiddleware>();
    }
}
