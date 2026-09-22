using System.Text.Json;
using System.Text.Json.Serialization;
using core.Services;
using server.Hubs;

var builder = WebApplication.CreateBuilder(args);

const string ClientCorsPolicy = "ClientCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        ClientCorsPolicy,
        policy =>
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
    );
});

builder
    .Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        );
    });
builder.Services.AddSingleton<GameManager>();
builder.Services.AddSingleton<GameConnectionRegistry>();

var app = builder.Build();

app.UseCors(ClientCorsPolicy);

app.MapGet("/", () => "Hello World!");
app.MapHub<GameHub>("/hubs/game");

app.Run();
