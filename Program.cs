var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<Repository>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
