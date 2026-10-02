using System.Collections.Concurrent;
using TicTacToe.Models;

namespace TicTacToe.Services;

public sealed class RoomStore
{
    private readonly ConcurrentDictionary<Guid, Room> rooms = [];

    public void Add(Room room)
    {
        if (!rooms.TryAdd(room.Id, room))
        {
            throw new InvalidOperationException("Room already exists");
        }
    }

    public void Remove(Guid roomId)
    {
        if (!rooms.TryRemove(roomId, out _))
        {
            throw new InvalidOperationException("Room doesn't exists");
        }
    }

    public Room? Get(Guid roomId)
    {
        return rooms.GetValueOrDefault(roomId);
    }

    public void Update(Guid roomId)
    {
        var room = rooms.GetValueOrDefault(roomId);
        if (room == null)
        {
            throw new InvalidOperationException("Room doesn't exists");
        }

        room.Game = new Game();
        rooms.TryUpdate(roomId, room, room);
    }
}