using System.Collections.Concurrent;
using Google.Protobuf;
using Grpc.Core;
using TicTacToe.Models;
using TicTacToe.Protos;

namespace TicTacToe.Services;

public class RoomService : Protos.RoomService.RoomServiceBase
{
    private readonly List<Room> rooms = [];
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    
    public override async Task<CreateRoomReply> CreateRoom(CreateRoomRequest request, ServerCallContext context)
    {
        await semaphore.WaitAsync();
        try
        {
            var id = Guid.NewGuid();
            var game = new Game()
            {
                Status = GameStatus.WaitingPlayer
            };
            var room = new Room(id, game);
            rooms.Add(room);

            return new CreateRoomReply
            {
                RoomId = ByteString.CopyFrom(id.ToByteArray())
            };
        }
        finally
        {
            semaphore.Release();
        }
    }

    public override async Task<JoinUserReply> JoinRoom(JoinUserRequest request, ServerCallContext context)
    {
        await semaphore.WaitAsync();
        try
        {
            var guid = new Guid(request.RoomId.ToByteArray());
            var currentRoom = rooms.FirstOrDefault(x => x.Id == guid);
            if (currentRoom == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Room not found"));
            }

            var playerId = new Guid();
            switch (currentRoom.Game.Players.Count)
            {
                case > 2:
                    throw new  RpcException(new Status(StatusCode.AlreadyExists, "You cannot join more than two player"));
                case > 1:
                    currentRoom.Game.Players.Add(playerId);
                    currentRoom.Game.Status = GameStatus.TurnX;
                    break;
                default:
                    currentRoom.Game.Players.Add(playerId);
                    break;
            }

            return new JoinUserReply()
            {
                UserId = ByteString.CopyFrom(playerId.ToByteArray())
            };
        }
        finally
        {
            semaphore.Release();
        }
    }
}