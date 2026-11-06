using TicTacToe.Web.Enums;

namespace TicTacToe.Web.Entities;

public class GameRoom
{
    public Guid Id { get; set; }
    public string RoomCode { get; set; } = string.Empty;
    public string Board { get; set; } = "---------";
    public GameStatus Status { get; set; } = GameStatus.WaitingForPlayer;
    public PlayerSymbol? CurrentTurn { get; set; }
    public PlayerSymbol? WinnerSymbol { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public ICollection<GamePlayer> Players { get; set; } = new List<GamePlayer>();
    public ICollection<GameResult> Results { get; set; } = new List<GameResult>();
}
