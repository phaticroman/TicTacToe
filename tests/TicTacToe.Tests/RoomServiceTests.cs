using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Data;
using TicTacToe.Web.Entities;
using TicTacToe.Web.Enums;
using TicTacToe.Web.Services;

namespace TicTacToe.Tests;

public class RoomServiceTests
{
    private static AppDbContext CreateContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateRoom_CreatesWaitingRoomWithCreatorAsX()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);

        RoomActionResult result = await service.CreateRoomAsync("Alice");

        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.NotNull(result.RoomId);
        Assert.Equal(6, result.RoomCode!.Length);
        Assert.NotNull(result.PlayerId);
        Assert.Equal(PlayerSymbol.X, result.Symbol);

        GameRoom room = await db.GameRooms.Include(r => r.Players).SingleAsync();
        Assert.Equal("---------", room.Board);
        Assert.Equal(GameStatus.WaitingForPlayer, room.Status);
        Assert.Null(room.CurrentTurn);
        Assert.Null(room.WinnerSymbol);

        GamePlayer player = Assert.Single(room.Players);
        Assert.Equal("Alice", player.DisplayName);
        Assert.Equal(PlayerSymbol.X, player.Symbol);
        Assert.False(player.IsConnected);
        Assert.False(player.WantsRematch);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateRoom_RejectsMissingDisplayName(string name)
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);

        RoomActionResult result = await service.CreateRoomAsync(name);

        Assert.False(result.Success);
        Assert.Equal("Enter a display name.", result.Error);
    }

    [Fact]
    public async Task CreateRoom_RejectsTooLongDisplayName()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);

        RoomActionResult result = await service.CreateRoomAsync(new string('A', 21));

        Assert.False(result.Success);
        Assert.Equal("Display name must be 20 characters or fewer.", result.Error);
    }

    [Fact]
    public async Task CreateRoom_TrimsDisplayName()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);

        await service.CreateRoomAsync("  Alice  ");

        GamePlayer player = await db.GamePlayers.SingleAsync();
        Assert.Equal("Alice", player.DisplayName);
    }

    [Fact]
    public async Task CreateRoom_GeneratesDistinctRoomCodes()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        var codes = new HashSet<string>();

        for (int i = 0; i < 25; i++)
        {
            RoomActionResult result = await service.CreateRoomAsync($"Player{i}");

            Assert.True(result.Success);
            Assert.True(codes.Add(result.RoomCode!));
        }
    }

    [Fact]
    public async Task JoinRoom_StartsGameAndAssignsO()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");

        RoomActionResult joined = await service.JoinRoomAsync(created.RoomCode!, "Bob");

        Assert.True(joined.Success, joined.Error);
        Assert.Null(joined.Error);
        Assert.Equal(created.RoomId, joined.RoomId);
        Assert.Equal(created.RoomCode, joined.RoomCode);
        Assert.Equal(PlayerSymbol.O, joined.Symbol);

        GameRoom room = await db.GameRooms.Include(r => r.Players).SingleAsync();
        Assert.Equal(GameStatus.InProgress, room.Status);
        Assert.Equal(PlayerSymbol.X, room.CurrentTurn);
        Assert.Equal(2, room.Players.Count);
        Assert.Contains(room.Players, p => p.DisplayName == "Bob" && p.Symbol == PlayerSymbol.O);
    }

    [Fact]
    public async Task JoinRoom_AcceptsLowercaseRoomCode()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");

        RoomActionResult joined = await service.JoinRoomAsync(created.RoomCode!.ToLowerInvariant(), "Bob");

        Assert.True(joined.Success, joined.Error);
    }

    [Fact]
    public async Task JoinRoom_RejectsUnknownRoom()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);

        RoomActionResult result = await service.JoinRoomAsync("ZZZZZZ", "Bob");

        Assert.False(result.Success);
        Assert.Equal("Room not found.", result.Error);
    }

    [Fact]
    public async Task JoinRoom_RejectsThirdPlayer()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");
        await service.JoinRoomAsync(created.RoomCode!, "Bob");

        RoomActionResult third = await service.JoinRoomAsync(created.RoomCode!, "Cara");

        Assert.False(third.Success);
        Assert.Equal("That room is full.", third.Error);
    }

    [Fact]
    public async Task JoinRoom_RejectsRoomNotWaitingForPlayer()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");

        GameRoom room = await db.GameRooms.SingleAsync();
        room.Status = GameStatus.Won;
        await db.SaveChangesAsync();

        RoomActionResult result = await service.JoinRoomAsync(created.RoomCode!, "Bob");

        Assert.False(result.Success);
        Assert.Equal("That room is not accepting players.", result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC")]
    [InlineData("ABCDEFG")]
    public async Task JoinRoom_RejectsInvalidRoomCode(string roomCode)
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);

        RoomActionResult result = await service.JoinRoomAsync(roomCode, "Bob");

        Assert.False(result.Success);
        Assert.Equal("Enter a 6-character room code.", result.Error);
    }

    [Fact]
    public async Task JoinRoom_RejectsMissingDisplayName()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");

        RoomActionResult result = await service.JoinRoomAsync(created.RoomCode!, "   ");

        Assert.False(result.Success);
        Assert.Equal("Enter a display name.", result.Error);
    }

    [Fact]
    public async Task GetRoom_ReturnsRoomWithPlayersOrderedBySymbol()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");
        await service.JoinRoomAsync(created.RoomCode!, "Bob");

        RoomInfo? room = await service.GetRoomAsync(created.RoomCode!.ToLowerInvariant());

        Assert.NotNull(room);
        Assert.Equal(created.RoomId, room!.RoomId);
        Assert.Equal(created.RoomCode, room.RoomCode);
        Assert.Equal("---------", room.Board);
        Assert.Equal(GameStatus.InProgress, room.Status);
        Assert.Equal(PlayerSymbol.X, room.CurrentTurn);
        Assert.Null(room.WinnerSymbol);
        Assert.Equal(2, room.Players.Count);
        Assert.Equal(PlayerSymbol.X, room.Players[0].Symbol);
        Assert.Equal("Alice", room.Players[0].DisplayName);
        Assert.Equal(PlayerSymbol.O, room.Players[1].Symbol);
        Assert.Equal("Bob", room.Players[1].DisplayName);
    }

    [Fact]
    public async Task GetRoom_ReturnsNullForUnknownCode()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);

        Assert.Null(await service.GetRoomAsync("ZZZZZZ"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC")]
    public async Task GetRoom_ReturnsNullForInvalidCode(string roomCode)
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);

        Assert.Null(await service.GetRoomAsync(roomCode));
    }

    [Fact]
    public async Task SetPlayerConnected_UpdatesPresenceAndReturnsRoom()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");
        await service.JoinRoomAsync(created.RoomCode!, "Bob");

        RoomInfo? connected = await service.SetPlayerConnectedAsync(
            created.RoomCode!,
            created.PlayerId!.Value,
            true);

        Assert.NotNull(connected);
        Assert.True(connected!.Players.Single(p => p.PlayerId == created.PlayerId).IsConnected);
        Assert.False(connected.Players.Single(p => p.PlayerId != created.PlayerId).IsConnected);

        RoomInfo? disconnected = await service.SetPlayerConnectedAsync(
            created.RoomCode!,
            created.PlayerId.Value,
            false);

        Assert.NotNull(disconnected);
        Assert.False(disconnected!.Players.Single(p => p.PlayerId == created.PlayerId).IsConnected);
    }

    [Fact]
    public async Task SetPlayerConnected_ReturnsNullForUnknownRoomOrPlayer()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");

        Assert.Null(await service.SetPlayerConnectedAsync("ZZZZZZ", created.PlayerId!.Value, true));
        Assert.Null(await service.SetPlayerConnectedAsync(created.RoomCode!, Guid.NewGuid(), true));
    }
}
