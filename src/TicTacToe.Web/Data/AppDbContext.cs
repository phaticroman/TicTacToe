using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Entities;

namespace TicTacToe.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<GameRoom> GameRooms => Set<GameRoom>();
    public DbSet<GamePlayer> GamePlayers => Set<GamePlayer>();
    public DbSet<GameResult> GameResults => Set<GameResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameRoom>(entity =>
        {
            entity.HasKey(room => room.Id);
            entity.Property(room => room.RoomCode).HasMaxLength(6).IsRequired();
            entity.HasIndex(room => room.RoomCode).IsUnique();
            entity.Property(room => room.Board).HasColumnType("char(9)").IsRequired();
            entity.Property(room => room.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(room => room.CurrentTurn).HasConversion<string>().HasColumnType("char(1)");
            entity.Property(room => room.WinnerSymbol).HasConversion<string>().HasColumnType("char(1)");
            entity.Property(room => room.CreatedAt).HasColumnType("datetime2");
            entity.Property(room => room.UpdatedAt).HasColumnType("datetime2");
            entity.Property(room => room.RowVersion).IsRowVersion();
            entity.ToTable("GameRooms", table =>
            {
                table.HasCheckConstraint("CK_GameRooms_Board", "LEN([Board]) = 9 AND [Board] NOT LIKE '%[^XO-]%'");
                table.HasCheckConstraint("CK_GameRooms_Status", "[Status] IN ('WaitingForPlayer', 'InProgress', 'Won', 'Draw')");
            });
            entity.HasMany(room => room.Players)
                .WithOne(player => player.Room)
                .HasForeignKey(player => player.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(room => room.Results)
                .WithOne(result => result.Room)
                .HasForeignKey(result => result.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GamePlayer>(entity =>
        {
            entity.HasKey(player => player.Id);
            entity.Property(player => player.DisplayName).HasMaxLength(20).IsRequired();
            entity.Property(player => player.Symbol).HasConversion<string>().HasColumnType("char(1)").IsRequired();
            entity.HasIndex(player => new { player.RoomId, player.Symbol }).IsUnique();
            entity.Property(player => player.JoinedAt).HasColumnType("datetime2");
            entity.ToTable("GamePlayers", table =>
            {
                table.HasCheckConstraint("CK_GamePlayers_Symbol", "[Symbol] IN ('X', 'O')");
            });
        });

        modelBuilder.Entity<GameResult>(entity =>
        {
            entity.HasKey(result => result.Id);
            entity.Property(result => result.RoundNumber).IsRequired();
            entity.Property(result => result.WinnerSymbol).HasConversion<string>().HasColumnType("char(1)");
            entity.Property(result => result.EndedAt).HasColumnType("datetime2");
            entity.HasIndex(result => new { result.RoomId, result.RoundNumber }).IsUnique();
            entity.ToTable("GameResults", table =>
            {
                table.HasCheckConstraint(
                    "CK_GameResults_WinnerSymbol",
                    "[WinnerSymbol] IS NULL OR [WinnerSymbol] IN ('X', 'O')");
            });
        });
    }
}
