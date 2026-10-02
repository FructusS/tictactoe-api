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
            error => throw ErrorMapper.MapCreateRoomError(error));
    }

    public override async Task<JoinUserReply> JoinRoom(JoinUserRequest request, ServerCallContext context)
    {
        var result = manager.JoinRoom(new Guid(request.RoomId.ToByteArray()));
        return result.Match(
            success => new JoinUserReply()
            {
                UserId = ByteString.CopyFrom(success.ToByteArray())
            },
            error => throw ErrorMapper.MapJoinRoomError(error)
        );
    }
}