namespace TicTacToe.Web.Services;

public sealed record GameActionResult(bool Success, string? Error, RoomInfo? Room)
{
    public static GameActionResult Fail(string error) =>
        new(false, error, null);

    public static GameActionResult Ok(RoomInfo room) =>
        new(true, null, room);
}
