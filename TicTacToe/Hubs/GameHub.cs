using Microsoft.AspNetCore.SignalR;
using System.IO.Pipelines;
using Microsoft.OpenApi.Extensions;
using TicTacToe.Models;

namespace TicTacToe.Hubs
{
    public class GameHub : Hub
    {

        private static readonly Dictionary<string, string> _roomDictionary = new Dictionary<string, string>();
        private static readonly Dictionary<string, Game> _gameDictionary = new Dictionary<string, Game>();

        public async Task MakeMove(string roomId, int x, int y, Piece piece)
        {

            await Clients.Groups(roomId).SendAsync("MadeMove", x, y, piece);

            if (_gameDictionary.TryGetValue(roomId, out Game game))
            {
                if (game.CurrentPlayer != Context.ConnectionId)
                {
                    return;
                }

                game.MakeMove(x, y, piece);
                game.SetGameStatus(piece == Piece.X ? GameStatus.TurnX : GameStatus.TurnY);
                game.SetCurrentPlayer(Context.ConnectionId);

            }
        }
        // made 100%
        public async Task JoinRoom(string roomId)
        {
            //Join first player
            if (_roomDictionary.ToLookup(x => x.Key == roomId).Count == 0 && !_roomDictionary.ContainsKey(Context.ConnectionId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                _gameDictionary.Add(roomId, new Game());
                _roomDictionary.Add(this.Context.ConnectionId, roomId);

                if (_gameDictionary.TryGetValue(roomId, out Game game))
                {
                    game.SetGameStatus(GameStatus.WaitingSecondPlayer);
                    await Clients.Group(roomId).SendAsync("GameStatus",game.GameStatus);
                }
            }
            //Join second player
            else if (_roomDictionary.ToLookup(x => x.Key == roomId).Count == 1 && !_roomDictionary.ContainsKey(Context.ConnectionId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                _roomDictionary.Add(this.Context.ConnectionId, roomId);
                if (_gameDictionary.TryGetValue(roomId, out Game game)) 
                {
                    game.SetGameStatus(GameStatus.TurnX);
                    await Clients.Group(roomId).SendAsync("GameStatus", game.GameStatus);
                }
            }

        }

        public async Task LeaveRoom(string roomId)
        {
            //if (_roomDictionary.ToLookup(x => x.Key == Context.ConnectionId).Count == 1)
            //{
            //    _roomDictionary.Remove(Context.ConnectionId);
            //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

            //}
            _roomDictionary.Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }

        public override Task OnConnectedAsync()
        {   
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _roomDictionary.Remove(this.Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
