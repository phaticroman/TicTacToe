using TicTacToe.Web.Enums;
using TicTacToe.Web.Game;

namespace TicTacToe.Web.Services;

public sealed class LocalGameState
{
    public string Board { get; set; } = GameRules.EmptyBoard;
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    public PlayerSymbol? CurrentTurn { get; set; } = PlayerSymbol.X;
    public PlayerSymbol? WinnerSymbol { get; set; }
    public int MatchesPlayed { get; set; }
    public int Draws { get; set; }
    public int XWins { get; set; }
    public int OWins { get; set; }
}
