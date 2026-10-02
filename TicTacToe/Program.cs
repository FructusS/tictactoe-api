using TicTacToe.Hubs;
using TicTacToe.Models;
using TicTacToe.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<RoomStore>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGrpcService<RoomService>();

app.MapGet("/room", () => Results.Ok(Guid.NewGuid()));
//
// app.MapPost("/game", async () =>
// {
//     var game = new Game();
//     return Results.Ok(game.Field);
// });


// app.UseCors(builder =>
// {
//     builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
// });

// app.UseHttpsRedirection();


app.Run();

public partial class Program; // for tests