using TicTacToe.Models;

namespace TicTacToe.Services;

public enum GameError
{
    NoError = 0,
}

public class GameManager
{
    private readonly GameSubscriptions subscriptions;

    public GameManager(GameSubscriptions subscriptions)
    {
        this.subscriptions = subscriptions;
    }
    
    public Result<Game, GameError> MakeMove(
        Game game,
        Guid playerId,
        int position)
    {
        // game.MakeMove(playerId, position);

        subscriptions.Publish(game.Id, game);

        return Result.Success(game);
    }
}