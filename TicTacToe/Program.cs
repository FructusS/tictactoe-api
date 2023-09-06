using Microsoft.AspNetCore.SignalR;
using TicTacToe.Hubs;
using TicTacToe.Models;

namespace TicTacToe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> userList = new Dictionary<string, string>();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

    

            app.MapControllers();
            app.MapPost("/room/{roomId}/{connectionId}", async (string roomId, string connectionId, IHubContext<GameHub> game) =>
            {
                var countUsersInGroup = userList.ToLookup(key => key.Key == roomId);
                if (countUsersInGroup.Count <= 1)
                {
                    await game.Groups.AddToGroupAsync(roomId, connectionId);
                    userList.Add(roomId, connectionId);
                    return Results.Ok("user successful added in group");
                }
                else
                {
                    return Results.BadRequest("Group 2 people");
                }
            });
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
            app.MapHub<GameHub>("/game");

            app.Run();
        }
    }
}