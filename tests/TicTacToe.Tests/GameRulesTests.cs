using TicTacToe.Web.Enums;
using TicTacToe.Web.Game;

namespace TicTacToe.Tests;

public class GameRulesTests
{
    [Fact]
    public void EmptyBoard_HasNineEmptyCells()
    {
        Assert.Equal("---------", GameRules.EmptyBoard);
        Assert.Equal(9, GameRules.EmptyBoard.Length);
    }

    [Fact]
    public void FirstMove_PlacesX_AndPassesTurnToO()
    {
        MoveResult result = GameRules.ApplyMove(
            GameRules.EmptyBoard,
            GameStatus.InProgress,
            PlayerSymbol.X,
            PlayerSymbol.X,
            4);

        Assert.True(result.IsValid);
        Assert.Null(result.Error);
        Assert.Equal("----X----", result.Board);
        Assert.Equal(GameStatus.InProgress, result.Status);
        Assert.Equal(PlayerSymbol.O, result.CurrentTurn);
        Assert.Null(result.Winner);
    }

    [Fact]
    public void SecondMove_PlacesO_AndPassesTurnToX()
    {
        MoveResult result = GameRules.ApplyMove(
            "----X----",
            GameStatus.InProgress,
            PlayerSymbol.O,
            PlayerSymbol.O,
            0);

        Assert.True(result.IsValid);
        Assert.Equal("O---X----", result.Board);
        Assert.Equal(GameStatus.InProgress, result.Status);
        Assert.Equal(PlayerSymbol.X, result.CurrentTurn);
        Assert.Null(result.Winner);
    }

    [Fact]
    public void OutOfTurnMove_IsRejected()
    {
        MoveResult result = GameRules.ApplyMove(
            GameRules.EmptyBoard,
            GameStatus.InProgress,
            PlayerSymbol.X,
            PlayerSymbol.O,
            0);

        Assert.False(result.IsValid);
        Assert.Equal("It is not your turn.", result.Error);
        Assert.Equal(GameRules.EmptyBoard, result.Board);
        Assert.Equal(PlayerSymbol.X, result.CurrentTurn);
    }

    [Fact]
    public void OccupiedCell_IsRejected()
    {
        MoveResult result = GameRules.ApplyMove(
            "----X----",
            GameStatus.InProgress,
            PlayerSymbol.O,
            PlayerSymbol.O,
            4);

        Assert.False(result.IsValid);
        Assert.Equal("That cell is already occupied.", result.Error);
        Assert.Equal("----X----", result.Board);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void CellOutsideBoard_IsRejected(int cellIndex)
    {
        MoveResult result = GameRules.ApplyMove(
            GameRules.EmptyBoard,
            GameStatus.InProgress,
            PlayerSymbol.X,
            PlayerSymbol.X,
            cellIndex);

        Assert.False(result.IsValid);
        Assert.Equal("Cell must be between 0 and 8.", result.Error);
    }

    [Theory]
    [InlineData(GameStatus.WaitingForPlayer)]
    [InlineData(GameStatus.Won)]
    [InlineData(GameStatus.Draw)]
    public void MoveWhenGameNotInProgress_IsRejected(GameStatus status)
    {
        MoveResult result = GameRules.ApplyMove(
            GameRules.EmptyBoard,
            status,
            PlayerSymbol.X,
            PlayerSymbol.X,
            0);

        Assert.False(result.IsValid);
        Assert.Equal("The game is not in progress.", result.Error);
    }

    [Fact]
    public void MoveWhenTurnMissing_IsRejected()
    {
        MoveResult result = GameRules.ApplyMove(
            GameRules.EmptyBoard,
            GameStatus.InProgress,
            null,
            PlayerSymbol.X,
            0);

        Assert.False(result.IsValid);
        Assert.Equal("The game has no active turn.", result.Error);
    }

    [Fact]
    public void MalformedBoard_IsRejected()
    {
        MoveResult result = GameRules.ApplyMove(
            "---",
            GameStatus.InProgress,
            PlayerSymbol.X,
            PlayerSymbol.X,
            0);

        Assert.False(result.IsValid);
        Assert.Equal("Board must contain exactly 9 cells.", result.Error);
    }

    [Theory]
    [InlineData("XXX------", PlayerSymbol.X)]
    [InlineData("---XXX---", PlayerSymbol.X)]
    [InlineData("------OOO", PlayerSymbol.O)]
    [InlineData("X--X--X--", PlayerSymbol.X)]
    [InlineData("-O--O--O-", PlayerSymbol.O)]
    [InlineData("--X--X--X", PlayerSymbol.X)]
    [InlineData("X---X---X", PlayerSymbol.X)]
    [InlineData("--O-O-O--", PlayerSymbol.O)]
    public void FindWinner_DetectsAllWinningLines(string board, PlayerSymbol expected)
    {
        Assert.Equal(expected, GameRules.FindWinner(board));
    }

    [Fact]
    public void FindWinner_ReturnsNull_WhenNoWinner()
    {
        Assert.Null(GameRules.FindWinner("XOX------"));
        Assert.Null(GameRules.FindWinner("XOXXOOOXX"));
    }

    [Fact]
    public void WinningMove_SetsWonStatusAndWinner()
    {
        MoveResult result = GameRules.ApplyMove(
            "XX-----O-",
            GameStatus.InProgress,
            PlayerSymbol.X,
            PlayerSymbol.X,
            2);

        Assert.True(result.IsValid);
        Assert.Null(result.Error);
        Assert.Equal("XXX----O-", result.Board);
        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(PlayerSymbol.X, result.Winner);
        Assert.Null(result.CurrentTurn);
    }

    [Fact]
    public void FillingLastCellWithoutWinner_IsDraw()
    {
        MoveResult result = GameRules.ApplyMove(
            "XOXXOOOX-",
            GameStatus.InProgress,
            PlayerSymbol.X,
            PlayerSymbol.X,
            8);

        Assert.True(result.IsValid);
        Assert.Null(result.Error);
        Assert.Equal("XOXXOOOXX", result.Board);
        Assert.Equal(GameStatus.Draw, result.Status);
        Assert.Null(result.Winner);
        Assert.Null(result.CurrentTurn);
    }

    [Fact]
    public void IsFull_DetectsFullBoard()
    {
        Assert.True(GameRules.IsFull("XOXXOOOXX"));
        Assert.False(GameRules.IsFull("XOXXOOOX-"));
        Assert.False(GameRules.IsFull("---"));
    }

    [Fact]
    public void Other_ReturnsOppositeSymbol()
    {
        Assert.Equal(PlayerSymbol.O, GameRules.Other(PlayerSymbol.X));
        Assert.Equal(PlayerSymbol.X, GameRules.Other(PlayerSymbol.O));
    }
}
