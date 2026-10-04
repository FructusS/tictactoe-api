using System.Collections.Concurrent;
using Google.Protobuf;
using TicTacToe.Models;
using TicTacToe.Protos;
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
    private readonly ILogger<RoomManager> logger;

    public RoomManager(ILogger<RoomManager> logger, RoomStore store)
    {
        this.logger = logger;
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
            logger.LogError(message: ex.Message, exception: ex);
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

        var piece = (Piece)Random.Shared.Next(1, 2);

        var pieceAlreadyExists = room.Game.Players.Any(x => x.SelectedPiece == piece);

        if (pieceAlreadyExists)
        {
            piece = piece switch
            {
                Piece.X => Piece.O,
                Piece.O => Piece.X,
                _ => throw new ArgumentOutOfRangeException(nameof(piece), piece, "Invalid piece.")
            };
        }
        
        room.Game.Players.Add(new Player()
        {
            PlayerId = ByteString.CopyFrom(playerId.ToByteArray()),
            SelectedPiece = piece
        });
        
        if (room.Game.Players.Count == 2)
        {
            room.Game.Status = GameStatus.TurnX;
        }
        
        return Result.Success(playerId);
    }
}