using System.Collections.Concurrent;
using Google.Protobuf;
using Grpc.Core;
using TicTacToe.Models;
using TicTacToe.Protos;

namespace TicTacToe.Services;

public class RoomService : Protos.RoomService.RoomServiceBase
{
    private readonly RoomStore store;

    public RoomService(RoomStore store)
    {
        this.store = store;
    }
    
    public override async Task<CreateRoomReply> CreateRoom(CreateRoomRequest request, ServerCallContext context)
    {
        var id = Guid.NewGuid();
        var game = new Game()
        {
            Status = GameStatus.WaitingPlayer
        };
        var room = new Room(id, game);
        
        store.Rooms.TryAdd(id, room);

        return new CreateRoomReply
        {
            RoomId = ByteString.CopyFrom(id.ToByteArray())
        };
    }

    public override async Task<JoinUserReply> JoinRoom(JoinUserRequest request, ServerCallContext context)
    {
        var guid = new Guid(request.RoomId.ToByteArray());
        
        if (!store.Rooms.TryGetValue(guid, out var currentRoom))
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Room not found"));
        }

        if (currentRoom.Game.Players.Count >= 2)
        {
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    "Room is full"));
        }

        var playerId = Guid.NewGuid();

        currentRoom.Game.Players.Add(playerId);

        if (currentRoom.Game.Players.Count == 2)
        {
            currentRoom.Game.Status = GameStatus.TurnX;
        }
        return new JoinUserReply()
        {
            UserId = ByteString.CopyFrom(playerId.ToByteArray())
        };
    }
}