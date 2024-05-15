using Api;
using Api.Endpoints;

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

app.UseMiddleware<UnhandledExceptionMiddleware>();

var apiGroup = app.MapGroup("api/v1");

app.RegisterModelEndpoints(apiGroup);

apiGroup.WithOpenApi();

// using (var scope = app.Services.CreateScope()) { await FunAndGames.TestStreamPrompt(scope, "Hello, i enjoy having fun!"); }
app.Run();