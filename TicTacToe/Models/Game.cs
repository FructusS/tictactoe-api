namespace TicTacToe.Models
{
    public class Game
    {
        public Piece[,] Field { get; private set; }

        public List<Guid> Players { get; private set; } = [];
        
        public Guid CurrentPlayer { get; set; }

        public GameStatus Status { get; set; }

        public Game()
        {
            Field = new Piece[3, 3];
        }

        public void SetCurrentPlayer(string connectionId)
        {
            //CurrentPlayer = connectionId;
        }

        public bool MakeMove(int x, int y, Piece piece)
        {
            
            if (Field[x, y] == Piece.Empty)
            {
                Field[x, y] = piece;
                return true;
            }
            return false;
        }

        public void SetGameStatus(GameStatus gameStatus)
        {
            Status = gameStatus;
        }
    }
}