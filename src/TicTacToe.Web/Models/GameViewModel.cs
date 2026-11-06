using TicTacToe.Web.Enums;
using TicTacToe.Web.Services;

namespace TicTacToe.Web.Models;

public class GameViewModel
{
    public required RoomInfo Room { get; init; }
    public required RoomPlayerInfo CurrentPlayer { get; init; }

    public bool IsWaitingForOpponent => Room.Players.Count < 2;

    public bool IsMyTurn =>
        Room.Status == GameStatus.InProgress && Room.CurrentTurn == CurrentPlayer.Symbol;

    public RoomPlayerInfo? Opponent =>
        Room.Players.FirstOrDefault(player => player.PlayerId != CurrentPlayer.PlayerId);

    public bool OpponentIsConnected => Opponent?.IsConnected == true;

    public bool IsGameOver => Room.Status is GameStatus.Won or GameStatus.Draw;

    public bool HasRequestedRematch => CurrentPlayer.WantsRematch;

    public bool OpponentWantsRematch => Opponent?.WantsRematch == true;

    public bool RematchButtonDisabled => HasRequestedRematch;

    public string RematchButtonText => HasRequestedRematch
        ? "Rematch requested"
        : OpponentWantsRematch
            ? "Accept rematch"
            : "Request rematch";

    public string RematchNote => HasRequestedRematch
        ? $"Waiting for {Opponent?.DisplayName ?? "the other player"} to accept."
        : OpponentWantsRematch
            ? $"{Opponent?.DisplayName ?? "The other player"} wants a rematch."
            : "Ready for another round?";

    public string StatusText => Room.Status switch
    {
        GameStatus.WaitingForPlayer => "Waiting for opponent",
        GameStatus.InProgress => IsMyTurn ? "Your turn" : "Opponent's turn",
        GameStatus.Won => Room.WinnerSymbol == CurrentPlayer.Symbol ? "You win" : "You lose",
        GameStatus.Draw => "Draw",
        _ => string.Empty
    };

    public string StatusClass => Room.Status switch
    {
        GameStatus.InProgress when IsMyTurn => "is-active",
        GameStatus.Won or GameStatus.Draw => "is-finished",
        _ => "is-waiting"
    };
}
