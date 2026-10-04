using Google.Protobuf;
using Grpc.Core;
using TicTacToe.Models;
using TicTacToe.Protos;

namespace TicTacToe.GrpcServices;

public class GameService : Protos.GameService.GameServiceBase
{
    private readonly GameSubscriptions _subscriptions;

    public GameService(GameSubscriptions subscriptions)
    {
        _subscriptions = subscriptions;
    }

    public override async Task SubscribeGame(
        SubscribeGameRequest request,
        IServerStreamWriter<GameUpdate> responseStream,
        ServerCallContext context)
    {
        var gameId = new Guid(
            request.GameId.ToByteArray());

        var (subscriptionId, reader) =
            _subscriptions.Subscribe(gameId);

        try
        {
            await foreach (
                var game in reader.ReadAllAsync(
                    context.CancellationToken))
            {
                await responseStream.WriteAsync(
                    ToProto(game));
            }
        }
        finally
        {
            _subscriptions.Unsubscribe(
                gameId,
                subscriptionId);
        }
    }

    public override Task<MakeMoveReply> MakeMove(MakeMoveRequest request, ServerCallContext context)
    {
        return base.MakeMove(request, context);
    }

    private static GameUpdate ToProto(Game game)
    {
        var response = new GameUpdate
        {
            GameId = ByteString.CopyFrom(
                game.Id.ToByteArray()),

            Status = game.Status
        };

        response.Players.AddRange(
            game.Players.Select(
                x => ByteString.CopyFrom(x.ToByteArray())));

        return response;
    }
}