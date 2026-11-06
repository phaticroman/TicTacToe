using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Data;
using TicTacToe.Web.Entities;
using TicTacToe.Web.Enums;
using TicTacToe.Web.Game;
using TicTacToe.Web.Services;

namespace TicTacToe.Tests;

public class GameServiceTests
{
    private static AppDbContext CreateContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<TestGame> CreateStartedGameAsync()
    {
        AppDbContext db = CreateContext();
        var roomService = new RoomService(db);
        RoomActionResult alice = await roomService.CreateRoomAsync("Alice");
        RoomActionResult bob = await roomService.JoinRoomAsync(alice.RoomCode!, "Bob");

        return new TestGame
        {
            Db = db,
            GameService = new GameService(db),
            RoomCode = alice.RoomCode!,
            XPlayerId = alice.PlayerId!.Value,
            OPlayerId = bob.PlayerId!.Value
        };
    }

    private static Task<GameActionResult> MoveAsync(TestGame game, Guid playerId, int cellIndex) =>
        game.GameService.MakeMoveAsync(game.RoomCode, playerId, cellIndex);

    [Fact]
    public async Task MakeMove_ValidMoveUpdatesBoardTurnAndRoomInfo()
    {
        using TestGame game = await CreateStartedGameAsync();

        GameActionResult result = await MoveAsync(game, game.XPlayerId, 0);

        Assert.True(result.Success, result.Error);
        Assert.Null(result.Error);
        RoomInfo room = Assert.IsType<RoomInfo>(result.Room);
        Assert.Equal("X--------", room.Board);
        Assert.Equal(GameStatus.InProgress, room.Status);
        Assert.Equal(PlayerSymbol.O, room.CurrentTurn);
        Assert.Null(room.WinnerSymbol);
        Assert.Equal(2, room.Players.Count);

        GameRoom stored = await game.Db.GameRooms.AsNoTracking().SingleAsync();
        Assert.Equal("X--------", stored.Board);
        Assert.Equal(PlayerSymbol.O, stored.CurrentTurn);
        Assert.Equal(GameStatus.InProgress, stored.Status);
    }

    [Fact]
    public async Task MakeMove_AcceptsLowercaseRoomCode()
    {
        using TestGame game = await CreateStartedGameAsync();

        GameActionResult result = await game.GameService.MakeMoveAsync(
            game.RoomCode.ToLowerInvariant(),
            game.XPlayerId,
            4);

        Assert.True(result.Success, result.Error);
        Assert.Equal("----X----", result.Room!.Board);
    }

    [Fact]
    public async Task MakeMove_RejectsOutOfTurnPlayer()
    {
        using TestGame game = await CreateStartedGameAsync();

        GameActionResult result = await MoveAsync(game, game.OPlayerId, 0);

        Assert.False(result.Success);
        Assert.Equal("It is not your turn.", result.Error);
        Assert.Null(result.Room);

        GameRoom stored = await game.Db.GameRooms.AsNoTracking().SingleAsync();
        Assert.Equal("---------", stored.Board);
        Assert.Equal(PlayerSymbol.X, stored.CurrentTurn);
    }

    [Fact]
    public async Task MakeMove_RejectsOccupiedCell()
    {
        using TestGame game = await CreateStartedGameAsync();
        await MoveAsync(game, game.XPlayerId, 0);

        GameActionResult result = await MoveAsync(game, game.OPlayerId, 0);

        Assert.False(result.Success);
        Assert.Equal("That cell is already occupied.", result.Error);

        GameRoom stored = await game.Db.GameRooms.AsNoTracking().SingleAsync();
        Assert.Equal("X--------", stored.Board);
        Assert.Equal(PlayerSymbol.O, stored.CurrentTurn);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public async Task MakeMove_RejectsInvalidCellIndex(int cellIndex)
    {
        using TestGame game = await CreateStartedGameAsync();

        GameActionResult result = await MoveAsync(game, game.XPlayerId, cellIndex);

        Assert.False(result.Success);
        Assert.Equal("Cell must be between 0 and 8.", result.Error);
    }

    [Fact]
    public async Task MakeMove_RejectsUnknownRoom()
    {
        using TestGame game = await CreateStartedGameAsync();

        GameActionResult result = await game.GameService.MakeMoveAsync(
            "ZZZZZZ",
            game.XPlayerId,
            0);

        Assert.False(result.Success);
        Assert.Equal("Room not found.", result.Error);
    }

    [Fact]
    public async Task MakeMove_RejectsPlayerNotInRoom()
    {
        using TestGame game = await CreateStartedGameAsync();

        GameActionResult result = await MoveAsync(game, Guid.NewGuid(), 0);

        Assert.False(result.Success);
        Assert.Equal("You are not part of this room.", result.Error);
    }

    [Fact]
    public async Task MakeMove_RejectsMoveBeforeOpponentJoins()
    {
        using AppDbContext db = CreateContext();
        var roomService = new RoomService(db);
        var gameService = new GameService(db);
        RoomActionResult alice = await roomService.CreateRoomAsync("Alice");

        GameActionResult result = await gameService.MakeMoveAsync(
            alice.RoomCode!,
            alice.PlayerId!.Value,
            0);

        Assert.False(result.Success);
        Assert.Equal("The game is not in progress.", result.Error);
    }

    [Fact]
    public async Task MakeMove_DetectsWinAndClosesTheGame()
    {
        using TestGame game = await CreateStartedGameAsync();
        await MoveAsync(game, game.XPlayerId, 0);
        await MoveAsync(game, game.OPlayerId, 3);
        await MoveAsync(game, game.XPlayerId, 1);
        await MoveAsync(game, game.OPlayerId, 4);

        GameActionResult result = await MoveAsync(game, game.XPlayerId, 2);

        Assert.True(result.Success, result.Error);
        RoomInfo room = result.Room!;
        Assert.Equal("XXXOO----", room.Board);
        Assert.Equal(GameStatus.Won, room.Status);
        Assert.Equal(PlayerSymbol.X, room.WinnerSymbol);
        Assert.Null(room.CurrentTurn);
        RoomHistoryInfo history = Assert.Single(room.History);
        Assert.Equal(1, history.RoundNumber);
        Assert.Equal(PlayerSymbol.X, history.WinnerSymbol);
        Assert.Equal("Alice", history.WinnerName);
        Assert.True(history.EndedAt <= DateTime.UtcNow);

        Assert.Equal(1, room.Stats.MatchesPlayed);
        Assert.Equal(0, room.Stats.Draws);
        Assert.Equal(1, room.Stats.Players.Single(p => p.PlayerId == game.XPlayerId).Wins);
        Assert.Equal(0, room.Stats.Players.Single(p => p.PlayerId == game.OPlayerId).Wins);

        GameActionResult afterWin = await MoveAsync(game, game.OPlayerId, 5);
        Assert.False(afterWin.Success);
        Assert.Equal("The game is not in progress.", afterWin.Error);
    }

    [Fact]
    public async Task MakeMove_DetectsDraw()
    {
        using TestGame game = await CreateStartedGameAsync();

        GameActionResult result = await FinishDrawAsync(game);

        Assert.True(result.Success, result.Error);
        RoomInfo room = result.Room!;
        Assert.Equal("XOXXOOOXX", room.Board);
        Assert.Equal(GameStatus.Draw, room.Status);
        Assert.Null(room.WinnerSymbol);
        Assert.Null(room.CurrentTurn);
        RoomHistoryInfo history = Assert.Single(room.History);
        Assert.Equal(1, history.RoundNumber);
        Assert.Null(history.WinnerSymbol);
        Assert.Null(history.WinnerName);

        Assert.Equal(1, room.Stats.MatchesPlayed);
        Assert.Equal(1, room.Stats.Draws);
        Assert.All(room.Stats.Players, p => Assert.Equal(0, p.Wins));
    }

    [Fact]
    public async Task RequestRematch_BeforeGameEndsIsRejected()
    {
        using TestGame game = await CreateStartedGameAsync();

        GameActionResult result = await game.GameService.RequestRematchAsync(
            game.RoomCode,
            game.XPlayerId);

        Assert.False(result.Success);
        Assert.Equal("A rematch is only available after the game ends.", result.Error);
    }

    [Fact]
    public async Task RequestRematch_FirstRequestWaitsForOpponent()
    {
        using TestGame game = await CreateStartedGameAsync();
        await FinishGameWithXWinAsync(game);

        GameActionResult result = await game.GameService.RequestRematchAsync(
            game.RoomCode,
            game.XPlayerId);

        Assert.True(result.Success, result.Error);
        RoomInfo room = result.Room!;
        Assert.Equal(GameStatus.Won, room.Status);
        Assert.True(room.Players.Single(p => p.PlayerId == game.XPlayerId).WantsRematch);
        Assert.False(room.Players.Single(p => p.PlayerId == game.OPlayerId).WantsRematch);
    }

    [Fact]
    public async Task RequestRematch_BothPlayersResetTheBoard()
    {
        using TestGame game = await CreateStartedGameAsync();
        await FinishGameWithXWinAsync(game);

        await game.GameService.RequestRematchAsync(game.RoomCode, game.XPlayerId);
        GameActionResult result = await game.GameService.RequestRematchAsync(
            game.RoomCode,
            game.OPlayerId);

        Assert.True(result.Success, result.Error);
        RoomInfo room = result.Room!;
        Assert.Equal(GameRules.EmptyBoard, room.Board);
        Assert.Equal(GameStatus.InProgress, room.Status);
        Assert.Equal(PlayerSymbol.X, room.CurrentTurn);
        Assert.Null(room.WinnerSymbol);
        Assert.All(room.Players, p => Assert.False(p.WantsRematch));
        Assert.Single(room.History);

        GameRoom stored = await game.Db.GameRooms.AsNoTracking().SingleAsync();
        Assert.Equal(GameRules.EmptyBoard, stored.Board);
        Assert.Equal(GameStatus.InProgress, stored.Status);
        Assert.Equal(PlayerSymbol.X, stored.CurrentTurn);
        Assert.Null(stored.WinnerSymbol);
    }

    [Fact]
    public async Task Rematch_KeepsHistoryAndRecordsTheNextRound()
    {
        using TestGame game = await CreateStartedGameAsync();
        await FinishGameWithXWinAsync(game);
        await game.GameService.RequestRematchAsync(game.RoomCode, game.XPlayerId);
        await game.GameService.RequestRematchAsync(game.RoomCode, game.OPlayerId);

        await FinishGameWithXWinAsync(game);

        RoomInfo? loaded = await new RoomService(game.Db).GetRoomAsync(game.RoomCode);
        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.History.Count);
        Assert.Equal(new[] { 1, 2 }, loaded.History.Select(item => item.RoundNumber));
        Assert.All(loaded.History, item => Assert.Equal(PlayerSymbol.X, item.WinnerSymbol));
    }

    [Fact]
    public async Task RoomStats_CountsWinsAndDrawsAcrossRematches()
    {
        using TestGame game = await CreateStartedGameAsync();
        await FinishGameWithXWinAsync(game);
        await game.GameService.RequestRematchAsync(game.RoomCode, game.XPlayerId);
        await game.GameService.RequestRematchAsync(game.RoomCode, game.OPlayerId);
        await FinishGameWithOWinAsync(game);
        await game.GameService.RequestRematchAsync(game.RoomCode, game.XPlayerId);
        await game.GameService.RequestRematchAsync(game.RoomCode, game.OPlayerId);
        await FinishDrawAsync(game);

        RoomInfo? loaded = await new RoomService(game.Db).GetRoomAsync(game.RoomCode);

        Assert.NotNull(loaded);
        Assert.Equal(3, loaded!.Stats.MatchesPlayed);
        Assert.Equal(1, loaded.Stats.Draws);
        Assert.Equal(1, loaded.Stats.Players.Single(p => p.PlayerId == game.XPlayerId).Wins);
        Assert.Equal(1, loaded.Stats.Players.Single(p => p.PlayerId == game.OPlayerId).Wins);
    }

    [Fact]
    public async Task RequestRematch_RejectsPlayerNotInRoom()
    {
        using TestGame game = await CreateStartedGameAsync();
        await FinishGameWithXWinAsync(game);

        GameActionResult result = await game.GameService.RequestRematchAsync(
            game.RoomCode,
            Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("You are not part of this room.", result.Error);
    }

    private static async Task FinishGameWithXWinAsync(TestGame game)
    {
        await MoveAsync(game, game.XPlayerId, 0);
        await MoveAsync(game, game.OPlayerId, 3);
        await MoveAsync(game, game.XPlayerId, 1);
        await MoveAsync(game, game.OPlayerId, 4);
        await MoveAsync(game, game.XPlayerId, 2);
    }

    private static async Task FinishGameWithOWinAsync(TestGame game)
    {
        await MoveAsync(game, game.XPlayerId, 0);
        await MoveAsync(game, game.OPlayerId, 3);
        await MoveAsync(game, game.XPlayerId, 1);
        await MoveAsync(game, game.OPlayerId, 4);
        await MoveAsync(game, game.XPlayerId, 8);
        await MoveAsync(game, game.OPlayerId, 5);
    }

    private static async Task<GameActionResult> FinishDrawAsync(TestGame game)
    {
        await MoveAsync(game, game.XPlayerId, 0);
        await MoveAsync(game, game.OPlayerId, 1);
        await MoveAsync(game, game.XPlayerId, 2);
        await MoveAsync(game, game.OPlayerId, 4);
        await MoveAsync(game, game.XPlayerId, 3);
        await MoveAsync(game, game.OPlayerId, 5);
        await MoveAsync(game, game.XPlayerId, 7);
        await MoveAsync(game, game.OPlayerId, 6);
        return await MoveAsync(game, game.XPlayerId, 8);
    }

    private sealed class TestGame : IDisposable
    {
        public required AppDbContext Db { get; init; }

        public required GameService GameService { get; init; }

        public required string RoomCode { get; init; }

        public required Guid XPlayerId { get; init; }

        public required Guid OPlayerId { get; init; }

        public void Dispose() => Db.Dispose();
    }
}
