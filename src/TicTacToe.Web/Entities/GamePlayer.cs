using TicTacToe.Web.Enums;

namespace TicTacToe.Web.Entities;

public class GamePlayer
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public GameRoom Room { get; set; } = default!;
    public string DisplayName { get; set; } = string.Empty;
    public PlayerSymbol Symbol { get; set; }
    public bool IsConnected { get; set; }
    public bool WantsRematch { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
