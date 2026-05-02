using NotificationService.Application.Contracts;
using NotificationService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers + Dapr support
builder.Services.AddControllers().AddDapr();

// Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register EmailService
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Required for Dapr
app.UseCloudEvents();

// Map controllers + subscriptions
app.MapControllers();
app.MapSubscribeHandler();

app.Run();