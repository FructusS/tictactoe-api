using System.Collections.Concurrent;
using System.Threading.Channels;
using TicTacToe.Models;

namespace TicTacToe;

public sealed class GameSubscriptions
{
    private readonly ConcurrentDictionary<
        Guid,
        ConcurrentDictionary<Guid, Channel<Game>>> _subscriptions = new();

    public (Guid Id, ChannelReader<Game> Reader) Subscribe(Guid gameId)
    {
        var id = Guid.NewGuid();

        var channel = Channel.CreateUnbounded<Game>();

        var gameSubscriptions = _subscriptions.GetOrAdd(
            gameId,
            _ => new ConcurrentDictionary<Guid, Channel<Game>>());

        gameSubscriptions[id] = channel;

        return (id, channel.Reader);
    }

    public void Unsubscribe(Guid gameId, Guid subscriptionId)
    {
        if (!_subscriptions.TryGetValue(gameId, out var subscriptions))
            return;

        if (subscriptions.TryRemove(subscriptionId, out var channel))
        {
            channel.Writer.TryComplete();
        }

        if (subscriptions.IsEmpty)
        {
            _subscriptions.TryRemove(gameId, out _);
        }
    }

    public void Publish(Guid gameId, Game game)
    {
        if (!_subscriptions.TryGetValue(gameId, out var subscriptions))
            return;

        foreach (var subscription in subscriptions.Values)
        {
            subscription.Writer.TryWrite(game);
        }
    }
}