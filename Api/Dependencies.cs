using System.Text.Json;
using Api.Middleware;
using Domain.Configuration;
using Implementation.Client;
using Implementation.Database;
using Implementation.Handler;
using Implementation.Json;
using Implementation.Json.Reader;
using Implementation.Repository;
using Implementation.Service;
using Interface.Client;
using Interface.Handler;
using Interface.Json;
using Interface.Repository;
using Interface.Service;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;

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
            .Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName))
            .Configure<DiscordOptions>(builder.Configuration.GetSection(DiscordOptions.SectionName))
            .Configure<CredentialsOptions>(builder.Configuration.GetSection(CredentialsOptions.SectionName))
            .Configure<OpenAiOptions>(builder.Configuration.GetSection(OpenAiOptions.SectionName))
            .Configure<AnthropicOptions>(builder.Configuration.GetSection(AnthropicOptions.SectionName))
            .Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(new LlmContentConverter());
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.MaxDepth = 30;
            });
        
        // Serilog Configuration
        builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            var discordConfiguration = builder.Configuration
                .GetSection(DiscordOptions.SectionName)
                .Get<DiscordOptions>()!;

            var id = ulong.Parse(discordConfiguration.WebhookId);
            loggerConfiguration
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Discord(id, discordConfiguration.WebhookToken, restrictedToMinimumLevel: LogEventLevel.Fatal)
                .ReadFrom.Configuration(hostingContext.Configuration);
        });
        
        // Json
        builder.Services
            .AddScoped<IStreamLineReader, StreamLineReader>();

        // Handler
        builder.Services
            .AddScoped<IModelHandler, ModelHandler>()
            .AddScoped<IPromptHandler, PromptHandler>()
            .AddScoped<IStreamPromptHandler, StreamPromptHandler>();
        
        // Repository
        builder.Services
            .AddScoped<ILlmModelRepository, LlmModelRepository>()
            .AddScoped<IPromptRepository, PromptRepository>();

        // Service
        builder.Services
            .AddScoped<ICacheService, CacheService>()
            .AddScoped<ILlmModelService, LlmModelService>()
            .AddScoped<ILlmApiKeyService, LlmDistributedApiKeyService>()
            .AddScoped<LargeLanguageModelService>()
            .AddScoped<ILargeLanguageModelService, TrackedLargeLanguageModelService>()
            .AddScoped<IHttpPromptStreamService, HttpPromptStreamService>();
        
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
        
        // Cache
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            var redisConfiguration = builder.Configuration
                .GetSection(RedisOptions.SectionName)
                .Get<RedisOptions>()!;

            options.Configuration = redisConfiguration.ConnectionString;
            options.InstanceName = redisConfiguration.InstanceName;
        });

        // Middleware
        builder.Services
            .AddHttpContextAccessor()
            .AddScoped<BasicAuthenticationHandler>()
            .AddScoped<UnhandledExceptionMiddleware>();
    }
}
