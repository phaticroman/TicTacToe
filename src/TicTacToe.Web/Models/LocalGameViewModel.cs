using TicTacToe.Web.Enums;
using TicTacToe.Web.Services;

namespace TicTacToe.Web.Models;

public sealed class LocalGameViewModel
{
    public required string Board { get; init; }
    public GameStatus Status { get; init; }
    public PlayerSymbol? CurrentTurn { get; init; }
    public PlayerSymbol? WinnerSymbol { get; init; }
    public int MatchesPlayed { get; init; }
    public int Draws { get; init; }
    public int XWins { get; init; }
    public int OWins { get; init; }

    public bool IsGameOver => Status is GameStatus.Won or GameStatus.Draw;

    public string StatusText => Status switch
    {
        GameStatus.InProgress when CurrentTurn is not null => $"Player {CurrentTurn}'s turn",
        GameStatus.Won => $"Player {WinnerSymbol} wins",
        GameStatus.Draw => "Draw",
        _ => "Ready to play"
    };

    public string StatusClass => Status switch
    {
        GameStatus.InProgress => "is-active",
        GameStatus.Won or GameStatus.Draw => "is-finished",
        _ => "is-waiting"
    };

    public string GameNote => Status switch
    {
        GameStatus.InProgress when CurrentTurn is not null =>
            $"Player {CurrentTurn} has the next move. Pass the device after each turn.",
        GameStatus.Won => $"Player {WinnerSymbol} wins this round.",
        GameStatus.Draw => "This round ended in a draw.",
        _ => "X always goes first."
    };

    public static LocalGameViewModel FromState(LocalGameState state) =>
        new()
        {
            Board = state.Board,
            Status = state.Status,
            CurrentTurn = state.CurrentTurn,
            WinnerSymbol = state.WinnerSymbol,
            MatchesPlayed = state.MatchesPlayed,
            Draws = state.Draws,
            XWins = state.XWins,
            OWins = state.OWins
        };
}
