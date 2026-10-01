using TicTacToe.Hubs;
using TicTacToe.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/room", () => Results.Ok(Guid.NewGuid()));

app.MapPost("/game", async () =>
{
    var game = new Game();
    return Results.Ok(game.Field);
});


app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();