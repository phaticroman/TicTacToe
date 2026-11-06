using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Data;
using TicTacToe.Web.Entities;
using TicTacToe.Web.Enums;
using TicTacToe.Web.Game;

namespace TicTacToe.Web.Services;

public class GameService
{
    private readonly AppDbContext _db;

    public GameService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GameActionResult> MakeMoveAsync(
        string roomCode,
        Guid playerId,
        int cellIndex,
        CancellationToken cancellationToken = default)
    {
        string normalizedCode = roomCode.Trim().ToUpperInvariant();

        GameRoom? room = await _db.GameRooms
            .Include(r => r.Players)
            .Include(r => r.Results)
            .FirstOrDefaultAsync(r => r.RoomCode == normalizedCode, cancellationToken);

        if (room is null)
        {
            return GameActionResult.Fail("Room not found.");
        }

        GamePlayer? player = room.Players.FirstOrDefault(p => p.Id == playerId);
        if (player is null)
        {
            return GameActionResult.Fail("You are not part of this room.");
        }

        MoveResult move = GameRules.ApplyMove(
            room.Board,
            room.Status,
            room.CurrentTurn,
            player.Symbol,
            cellIndex);

        if (!move.IsValid)
        {
            return GameActionResult.Fail(move.Error ?? "That move is not allowed.");
        }

        DateTime now = DateTime.UtcNow;
        room.Board = move.Board;
        room.Status = move.Status;
        room.CurrentTurn = move.CurrentTurn;
        room.WinnerSymbol = move.Winner;
        room.UpdatedAt = now;

        if (move.Status is GameStatus.Won or GameStatus.Draw)
        {
            room.Results.Add(new GameResult
            {
                RoomId = room.Id,
                RoundNumber = room.Results.Count == 0 ? 1 : room.Results.Max(result => result.RoundNumber) + 1,
                WinnerSymbol = move.Winner,
                EndedAt = now
            });
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return GameActionResult.Fail("The board changed. Wait a moment and try again.");
        }

        return GameActionResult.Ok(RoomInfo.FromRoom(room));
    }

    public async Task<GameActionResult> RequestRematchAsync(
        string roomCode,
        Guid playerId,
        CancellationToken cancellationToken = default)
    {
        string normalizedCode = roomCode.Trim().ToUpperInvariant();

        GameRoom? room = await _db.GameRooms
            .Include(r => r.Players)
            .Include(r => r.Results)
            .FirstOrDefaultAsync(r => r.RoomCode == normalizedCode, cancellationToken);

        if (room is null)
        {
            return GameActionResult.Fail("Room not found.");
        }

        GamePlayer? player = room.Players.FirstOrDefault(p => p.Id == playerId);
        if (player is null)
        {
            return GameActionResult.Fail("You are not part of this room.");
        }

        if (room.Status is not (GameStatus.Won or GameStatus.Draw))
        {
            return GameActionResult.Fail("A rematch is only available after the game ends.");
        }

        if (room.Players.Count < 2)
        {
            return GameActionResult.Fail("A rematch needs both players.");
        }

        player.WantsRematch = true;

        if (room.Players.All(p => p.WantsRematch))
        {
            room.Board = GameRules.EmptyBoard;
            room.Status = GameStatus.InProgress;
            room.CurrentTurn = PlayerSymbol.X;
            room.WinnerSymbol = null;
            room.UpdatedAt = DateTime.UtcNow;

            foreach (GamePlayer rematchPlayer in room.Players)
            {
                rematchPlayer.WantsRematch = false;
            }
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return GameActionResult.Fail("The game changed. Wait a moment and try again.");
        }

        return GameActionResult.Ok(RoomInfo.FromRoom(room));
    }
}
