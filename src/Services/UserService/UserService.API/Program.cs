using Dapr.Client;
using UserService.Application.Contracts;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers for the HTTP API.
builder.Services.AddControllers();

// Enable Swagger/OpenAPI so you can test manually in the browser.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Dapr client so the service can publish integration events.
builder.Services.AddSingleton<DaprClient>(_ => new DaprClientBuilder().Build());

// Register the SQL Server connection factory used by Dapper.
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

// Register the repository implementation.
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Swagger is enabled only during local development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
