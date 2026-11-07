using TicTacToe.Web.Enums;
using TicTacToe.Web.Game;
using TicTacToe.Web.Services;

namespace TicTacToe.Tests;

public class LocalGameServiceTests
{
    private readonly LocalGameService _service = new();

    [Fact]
    public void ApplyMove_FirstMove_PlacesXAndPassesTurn()
    {
        LocalGameState state = new();

        MoveResult result = _service.ApplyMove(state, 4);

        Assert.True(result.IsValid);
        Assert.Equal("----X----", state.Board);
        Assert.Equal(GameStatus.InProgress, state.Status);
        Assert.Equal(PlayerSymbol.O, state.CurrentTurn);
        Assert.Null(state.WinnerSymbol);
        Assert.Equal(0, state.MatchesPlayed);
    }

    [Fact]
    public void ApplyMove_WinningMove_UpdatesScore()
    {
        LocalGameState state = new()
        {
            Board = "XX-----O-",
            CurrentTurn = PlayerSymbol.X
        };

        MoveResult result = _service.ApplyMove(state, 2);

        Assert.True(result.IsValid);
        Assert.Equal("XXX----O-", state.Board);
        Assert.Equal(GameStatus.Won, state.Status);
        Assert.Equal(PlayerSymbol.X, state.WinnerSymbol);
        Assert.Null(state.CurrentTurn);
        Assert.Equal(1, state.MatchesPlayed);
        Assert.Equal(1, state.XWins);
        Assert.Equal(0, state.OWins);
        Assert.Equal(0, state.Draws);
    }

    [Fact]
    public void ApplyMove_Draw_UpdatesDrawScore()
    {
        LocalGameState state = new()
        {
            Board = "XOXXOOOX-",
            CurrentTurn = PlayerSymbol.X
        };

        MoveResult result = _service.ApplyMove(state, 8);

        Assert.True(result.IsValid);
        Assert.Equal("XOXXOOOXX", state.Board);
        Assert.Equal(GameStatus.Draw, state.Status);
        Assert.Null(state.WinnerSymbol);
        Assert.Equal(1, state.MatchesPlayed);
        Assert.Equal(1, state.Draws);
        Assert.Equal(0, state.XWins);
        Assert.Equal(0, state.OWins);
    }

    [Fact]
    public void ApplyMove_OccupiedCell_LeavesStateUnchanged()
    {
        LocalGameState state = new()
        {
            Board = "----X----",
            CurrentTurn = PlayerSymbol.O
        };

        MoveResult result = _service.ApplyMove(state, 4);

        Assert.False(result.IsValid);
        Assert.Equal("That cell is already occupied.", result.Error);
        Assert.Equal("----X----", state.Board);
        Assert.Equal(PlayerSymbol.O, state.CurrentTurn);
        Assert.Equal(0, state.MatchesPlayed);
    }

    [Fact]
    public void StartNewRound_ResetsBoardAndKeepsScore()
    {
        LocalGameState state = new()
        {
            Board = "XXX----O-",
            Status = GameStatus.Won,
            CurrentTurn = null,
            WinnerSymbol = PlayerSymbol.X,
            MatchesPlayed = 3,
            XWins = 2,
            OWins = 0,
            Draws = 1
        };

        _service.StartNewRound(state);

        Assert.Equal(GameRules.EmptyBoard, state.Board);
        Assert.Equal(GameStatus.InProgress, state.Status);
        Assert.Equal(PlayerSymbol.X, state.CurrentTurn);
        Assert.Null(state.WinnerSymbol);
        Assert.Equal(3, state.MatchesPlayed);
        Assert.Equal(2, state.XWins);
        Assert.Equal(1, state.Draws);
    }
}
