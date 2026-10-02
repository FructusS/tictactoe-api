using System.Collections.Concurrent;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe;

public enum JoinRoomError
{
    RoomNotFound,
    RoomFull
}
public enum CreateRoomError
{
    RoomAlreadyExists
}

public class RoomManager
{
    private readonly RoomStore store;

    public RoomManager(RoomStore store)
    {
        this.store = store;
    }

    public Result<Room, CreateRoomError> CreateRoom()
    { 
        try
        {
            var id = Guid.NewGuid();
            var game = new Game()
            {
                Status = GameStatus.WaitingPlayer
            };
            var room = new Room(id, game);

            store.Add(room);

            return Result.Success(room);
        }
        catch (InvalidOperationException e)
        {
            return Result.Failure(CreateRoomError.RoomAlreadyExists);
        }
        catch (Exception ex)
        {
            // todo log here
            throw;
        }
    }
    
    public Result<Guid, JoinRoomError> JoinRoom(Guid roomId)
    {
        var room = store.Get(roomId);

        if (room == null)
        {
            return Result.Failure(JoinRoomError.RoomNotFound);
        }
        if (room.Game.Players.Count >= 2)
        {
           return Result.Failure(JoinRoomError.RoomFull);
        }

        var playerId = Guid.NewGuid();

        room.Game.Players.Add(playerId);

        if (room.Game.Players.Count == 2)
        {
            room.Game.Status = GameStatus.TurnX;
        }
        
        return Result.Success(playerId);
    }
}