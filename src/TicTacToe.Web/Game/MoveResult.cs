using TicTacToe.Web.Enums;

namespace TicTacToe.Web.Game;

public sealed record MoveResult(
    bool IsValid,
    string? Error,
    string Board,
    GameStatus Status,
    PlayerSymbol? CurrentTurn,
    PlayerSymbol? Winner);
