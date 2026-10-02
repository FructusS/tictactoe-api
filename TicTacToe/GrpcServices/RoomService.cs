using Google.Protobuf;
using Grpc.Core;
using TicTacToe.Models;
using TicTacToe.Protos;
using TicTacToe.Services;

namespace TicTacToe.GrpcServices;

public class RoomService : Protos.RoomService.RoomServiceBase
{
    private readonly ILogger<RoomService> logger;
    private readonly RoomManager manager;
    
    public RoomService(RoomManager manager)
    {
        this.manager = manager;
    }
    
    public override async Task<CreateRoomReply> CreateRoom(CreateRoomRequest request, ServerCallContext context)
    {
        var result = manager.CreateRoom();
        return result.Match(
            success => new CreateRoomReply
            {
                RoomId = ByteString.CopyFrom(success.Id.ToByteArray())
            },
            error => throw new RpcException(
                new Status(StatusCode.Internal, "Internal Server Error")));
    }

    public override async Task<JoinUserReply> JoinRoom(JoinUserRequest request, ServerCallContext context)
    {
      
        return new JoinUserReply()
        {
            UserId = ByteString.CopyFrom(playerId.ToByteArray())
        };
    }
}