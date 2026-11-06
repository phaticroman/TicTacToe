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
    IReadOnlyList<RoomPlayerInfo> Players,
    IReadOnlyList<RoomHistoryInfo> History,
    RoomStatsInfo Stats)
{
    public static RoomInfo FromRoom(GameRoom room)
    {
        List<RoomPlayerInfo> players = room.Players
            .OrderBy(p => p.Symbol)
            .Select(p => new RoomPlayerInfo(p.Id, p.DisplayName, p.Symbol, p.IsConnected, p.WantsRematch))
            .ToList();

        List<RoomHistoryInfo> history = room.Results
            .OrderBy(r => r.RoundNumber)
            .Select(r => new RoomHistoryInfo(
                r.RoundNumber,
                r.WinnerSymbol,
                r.WinnerSymbol is null
                    ? null
                    : room.Players.FirstOrDefault(p => p.Symbol == r.WinnerSymbol)?.DisplayName,
                AsUtc(r.EndedAt)))
            .ToList();

        int draws = history.Count(item => item.WinnerSymbol is null);
        List<RoomPlayerTallyInfo> playerTallies = players
            .Select(p => new RoomPlayerTallyInfo(
                p.PlayerId,
                p.DisplayName,
                p.Symbol,
                history.Count(item => item.WinnerSymbol == p.Symbol)))
            .ToList();

        return new RoomInfo(
            room.Id,
            room.RoomCode,
            room.Board,
            room.Status,
            room.CurrentTurn,
            room.WinnerSymbol,
            players,
            history,
            new RoomStatsInfo(history.Count, draws, playerTallies));
    }

    private static DateTime AsUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}

public sealed record RoomPlayerInfo(
    Guid PlayerId,
    string DisplayName,
    PlayerSymbol Symbol,
    bool IsConnected,
    bool WantsRematch);

public sealed record RoomHistoryInfo(
    int RoundNumber,
    PlayerSymbol? WinnerSymbol,
    string? WinnerName,
    DateTime EndedAt);

public sealed record RoomStatsInfo(
    int MatchesPlayed,
    int Draws,
    IReadOnlyList<RoomPlayerTallyInfo> Players);

public sealed record RoomPlayerTallyInfo(
    Guid PlayerId,
    string DisplayName,
    PlayerSymbol Symbol,
    int Wins);
