using TicTacToe.Web.Enums;
using TicTacToe.Web.Game;

namespace TicTacToe.Web.Services;

public sealed class LocalGameService
{
    public MoveResult ApplyMove(LocalGameState state, int cellIndex)
    {
        if (state.CurrentTurn is null)
        {
            return new MoveResult(
                false,
                "The game has no active turn.",
                state.Board,
                state.Status,
                state.CurrentTurn,
                null);
        }

        MoveResult move = GameRules.ApplyMove(
            state.Board,
            state.Status,
            state.CurrentTurn,
            state.CurrentTurn.Value,
            cellIndex);

        if (!move.IsValid)
        {
            return move;
        }

        state.Board = move.Board;
        state.Status = move.Status;
        state.CurrentTurn = move.CurrentTurn;
        state.WinnerSymbol = move.Winner;

        if (state.Status is GameStatus.Won or GameStatus.Draw)
        {
            state.MatchesPlayed++;

            if (state.Status == GameStatus.Draw)
            {
                state.Draws++;
            }
            else if (state.WinnerSymbol == PlayerSymbol.X)
            {
                state.XWins++;
            }
            else if (state.WinnerSymbol == PlayerSymbol.O)
            {
                state.OWins++;
            }
        }

        return move;
    }

    public void StartNewRound(LocalGameState state)
    {
        state.Board = GameRules.EmptyBoard;
        state.Status = GameStatus.InProgress;
        state.CurrentTurn = PlayerSymbol.X;
        state.WinnerSymbol = null;
    }
}
