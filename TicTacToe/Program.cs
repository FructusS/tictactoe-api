using Serilog;
using TicTacToe;
using TicTacToe.Services;
using GameService = TicTacToe.GrpcServices.GameService;
using RoomService = TicTacToe.GrpcServices.RoomService;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting server.");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

builder.Services.AddGrpc();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<RoomManager>();
builder.Services.AddSingleton<RoomStore>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGrpcService<RoomService>();
app.MapGrpcService<GameService>();

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