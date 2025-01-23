using CSDS_Assign_1;

// Initializes the WebApplication builder with the given arguments, setting up the configuration, logging, and DI container.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Registers the 'Repository' service with a scoped lifetime, meaning a new instance is created per request within the scope.
builder.Services.AddScoped<Repository>();

builder.Services.AddSingleton<ModeratedQuizService>();

// Adds support for MVC controllers to the service container, enabling routing and handling HTTP requests.
builder.Services.AddControllers();

// Builds the WebApplication pipeline from the configured services and middleware.
var app = builder.Build();

// Configure the HTTP request pipeline.

// Enables serving static files (e.g., HTML, CSS, JavaScript) from the wwwroot folder or a specified location.
app.UseStaticFiles();

// Adds middleware to handle user authorization, ensuring only authorized users can access specific resources.
app.UseAuthorization();

// Maps controller endpoints, connecting the routes defined in controllers to the application's request pipeline.
app.MapControllers();

app.UseWebSockets();

app.MapGet("/websockets/quiz", async (HttpContext context, ModeratedQuizService quizService) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await quizService.HandleWebSocketConnection(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Expected a WebSocket request");
    }
});

// Starts the application, listening for incoming HTTP requests.
app.Run();



