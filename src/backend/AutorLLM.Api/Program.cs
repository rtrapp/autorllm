using AutorLLM.Application;
using AutorLLM.Infrastructure;
using AutorLLM.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add SignalR
builder.Services.AddSignalR();

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add Application layer (MediatR, Validators)
builder.Services.AddApplication();

// Add Infrastructure layer (Repositories, UnitOfWork, AgentFramework)
builder.Services.AddInfrastructure(connectionString, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "AutorLLM API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

// Map SignalR Hub
app.MapHub<LLMHub>("/llmhub");

app.Run();
