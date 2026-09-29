using System.Text.Json;
using System.Text.Json.Serialization;
using core.Data;
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
// Send enums as "red", "locomotive" etc. - same as the SignalR hub does
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    );
});
builder.Services.AddSingleton<GameManager>();
builder.Services.AddSingleton<GameConnectionRegistry>();

var app = builder.Build();

app.UseCors(ClientCorsPolicy);

app.MapGet("/", () => "Hello World!");
app.MapGet(
    "/api/board",
    () => new { PlaceholderBoard.Cities, PlaceholderBoard.Routes }
);
app.MapHub<GameHub>("/hubs/game");

app.Run();
