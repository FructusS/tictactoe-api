using Grpc.Core;

namespace TicTacToe;

public static class ErrorMapper
{
    public static RpcException MapCreateRoomError(CreateRoomError error)
    {
        return error switch
        {
            CreateRoomError.RoomAlreadyExists => new RpcException(new Status(StatusCode.AlreadyExists,
                $"Room already exists")),
            _ =>
                new RpcException(
                    new Status(StatusCode.Internal, "Unknown error"))
        };
    }

    public static RpcException MapJoinRoomError(JoinRoomError error)
    {
        return error switch
        {
            JoinRoomError.RoomNotFound => new RpcException(new Status(StatusCode.NotFound, "Room not found")),
            JoinRoomError.RoomFull => new RpcException(new Status(StatusCode.FailedPrecondition, "Room is full")),
            _ =>
                new RpcException(
                    new Status(StatusCode.Internal, "Unknown error"))
        };
    }
}