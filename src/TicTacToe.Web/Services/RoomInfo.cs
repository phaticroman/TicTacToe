using TicTacToe.Web.Entities;
using TicTacToe.Web.Enums;

namespace TicTacToe.Web.Services;

public sealed record RoomInfo(
    Guid RoomId,
    string RoomCode,
    string Board,
    GameStatus Status,
    PlayerSymbol? CurrentTurn,
    PlayerSymbol? WinnerSymbol,
    IReadOnlyList<RoomPlayerInfo> Players)
{
    public static RoomInfo FromRoom(GameRoom room)
    {
        List<RoomPlayerInfo> players = room.Players
            .OrderBy(p => p.Symbol)
            .Select(p => new RoomPlayerInfo(p.Id, p.DisplayName, p.Symbol, p.IsConnected, p.WantsRematch))
            .ToList();

        return new RoomInfo(
            room.Id,
            room.RoomCode,
            room.Board,
            room.Status,
            room.CurrentTurn,
            room.WinnerSymbol,
            players);
    }
}

public sealed record RoomPlayerInfo(
    Guid PlayerId,
    string DisplayName,
    PlayerSymbol Symbol,
    bool IsConnected,
    bool WantsRematch);
