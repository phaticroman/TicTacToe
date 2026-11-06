using TicTacToe.Web.Enums;

namespace TicTacToe.Web.Entities;

public class GameResult
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public GameRoom Room { get; set; } = default!;
    public int RoundNumber { get; set; }
    public PlayerSymbol? WinnerSymbol { get; set; }
    public DateTime EndedAt { get; set; } = DateTime.UtcNow;
}
