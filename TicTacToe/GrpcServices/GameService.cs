using Grpc.Core;
using TicTacToe.Protos;

namespace TicTacToe.GrpcServices;

public class GameService : Protos.GameService.GameServiceBase
{
    public override Task MakeMove(IAsyncStreamReader<MakeMoveRequest> requestStream, IServerStreamWriter<MakeMoveReply> responseStream, ServerCallContext context)
    {
        return base.MakeMove(requestStream, responseStream, context);
    }
}