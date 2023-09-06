namespace TicTacToe.Models
{
    public class Game
    {
        public Piece[,] Field { get; set; }
        public string CurrentPlayer { get; private set; }

        public GameStatus GameStatus { get; private set; }

        public Game()
        {
            Field = new Piece[3, 3];
        }

        public void SetCurrentPlayer(string connectionId)
        {
            CurrentPlayer = connectionId;
        }

        public void MakeMove(int x, int y, Piece piece)
        {
            Field[x,y] = piece;
        }

        public void SetGameStatus(GameStatus gameStatus)
        {
            GameStatus = gameStatus;
        }
    }
}
