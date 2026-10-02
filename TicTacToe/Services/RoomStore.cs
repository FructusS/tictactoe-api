using System.Collections.Concurrent;
using TicTacToe.Models;

namespace TicTacToe.Services;

public sealed class RoomStore
{
    public ConcurrentDictionary<Guid, Room> Rooms = [];
}