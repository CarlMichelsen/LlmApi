using Api;
using Api.Extensions;
using Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterApplicationDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // "I also like to live dangerously" - Austin Powers
    await app.Services.EnsureDatabaseUpdated();
}

app.UseMiddleware<UnhandledExceptionMiddleware>();

app.UseMiddleware<BasicAuthenticationHandler>();

app.UseApplicationEndpoints();

app.Run();