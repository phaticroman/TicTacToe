using TicTacToe.Web.Enums;

namespace TicTacToe.Web.Game;

public static class GameRules
{
    public const string EmptyBoard = "---------";

    private static readonly int[][] WinningLines =
    [
        [0, 1, 2],
        [3, 4, 5],
        [6, 7, 8],
        [0, 3, 6],
        [1, 4, 7],
        [2, 5, 8],
        [0, 4, 8],
        [2, 4, 6]
    ];

    public static MoveResult ApplyMove(
        string board,
        GameStatus status,
        PlayerSymbol? currentTurn,
        PlayerSymbol player,
        int cellIndex)
    {
        if (status != GameStatus.InProgress)
        {
            return Invalid(board, status, currentTurn, "The game is not in progress.");
        }

        if (currentTurn is null)
        {
            return Invalid(board, status, currentTurn, "The game has no active turn.");
        }

        if (player != currentTurn)
        {
            return Invalid(board, status, currentTurn, "It is not your turn.");
        }

        if (board.Length != 9)
        {
            return Invalid(board, status, currentTurn, "Board must contain exactly 9 cells.");
        }

        if (cellIndex < 0 || cellIndex >= 9)
        {
            return Invalid(board, status, currentTurn, "Cell must be between 0 and 8.");
        }

        if (board[cellIndex] != '-')
        {
            return Invalid(board, status, currentTurn, "That cell is already occupied.");
        }

        char[] cells = board.ToCharArray();
        cells[cellIndex] = ToChar(player);
        string newBoard = new(cells);

        PlayerSymbol? winner = FindWinner(newBoard);
        if (winner is not null)
        {
            return new MoveResult(true, null, newBoard, GameStatus.Won, null, winner);
        }

        if (IsFull(newBoard))
        {
            return new MoveResult(true, null, newBoard, GameStatus.Draw, null, null);
        }

        return new MoveResult(true, null, newBoard, GameStatus.InProgress, Other(player), null);
    }

    public static PlayerSymbol? FindWinner(string board)
    {
        if (board.Length != 9)
        {
            return null;
        }

        foreach (int[] line in WinningLines)
        {
            char first = board[line[0]];
            if (first is not ('X' or 'O'))
            {
                continue;
            }

            if (first == board[line[1]] && first == board[line[2]])
            {
                return first == 'X' ? PlayerSymbol.X : PlayerSymbol.O;
            }
        }

        return null;
    }

    public static bool IsFull(string board) => board.Length == 9 && !board.Contains('-');

    public static PlayerSymbol Other(PlayerSymbol symbol) =>
        symbol == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;

    private static char ToChar(PlayerSymbol symbol) => symbol == PlayerSymbol.X ? 'X' : 'O';

    private static MoveResult Invalid(
        string board,
        GameStatus status,
        PlayerSymbol? currentTurn,
        string error) =>
        new(false, error, board, status, currentTurn, null);
}
