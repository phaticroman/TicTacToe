using TicTacToe.Web.Enums;

namespace TicTacToe.Web.Services;

public sealed record RoomActionResult(
    bool Success,
    string? Error,
    Guid? RoomId,
    string? RoomCode,
    Guid? PlayerId,
    PlayerSymbol? Symbol)
{
    public static RoomActionResult Fail(string error) =>
        new(false, error, null, null, null, null);

    public static RoomActionResult Ok(Guid roomId, string roomCode, Guid playerId, PlayerSymbol symbol) =>
        new(true, null, roomId, roomCode, playerId, symbol);
}
