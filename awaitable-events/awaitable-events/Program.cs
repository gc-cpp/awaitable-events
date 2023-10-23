using awaitable_events;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ConnectionService>();
var app = builder.Build();


app.MapPost("/connect/{id}", async (string id, ConnectionService connectionService, CancellationToken cancellationToken) =>
{
    await connectionService.ConnectAsync(id, cancellationToken);
});

app.MapPost("/v2/connect/{id}", async (string id, ConnectionService connectionService, CancellationToken cancellationToken) =>
{
    await connectionService.ConnectAsyncV2(id, cancellationToken);
});

app.Run();
