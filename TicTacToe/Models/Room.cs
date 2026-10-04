using TicTacToe.Protos;

namespace TicTacToe.Models;

public class Room(Guid id, Game game)
{
    public Guid Id { get; set; } = id;
    public Game Game { get; set; } = game; 
    public List<Player> Players { get; private set; } = [];
}