using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Data;
using TicTacToe.Web.Entities;
using TicTacToe.Web.Enums;
using TicTacToe.Web.Game;

namespace TicTacToe.Web.Services;

public class RoomService
{
    private const int RoomCodeLength = 6;
    private const int MaxDisplayNameLength = 20;
    private const int MaxCodeGenerationAttempts = 20;
    private const string RoomCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    private readonly AppDbContext _db;

    public RoomService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<RoomActionResult> CreateRoomAsync(
        string displayName,
        CancellationToken cancellationToken = default)
    {
        string? nameError = ValidateDisplayName(displayName);
        if (nameError is not null)
        {
            return RoomActionResult.Fail(nameError);
        }

        string? roomCode = await GenerateUniqueRoomCodeAsync(cancellationToken);
        if (roomCode is null)
        {
            return RoomActionResult.Fail("Could not create a room right now. Please try again.");
        }

        DateTime now = DateTime.UtcNow;
        var room = new GameRoom
        {
            RoomCode = roomCode,
            Board = GameRules.EmptyBoard,
            Status = GameStatus.WaitingForPlayer,
            CurrentTurn = null,
            WinnerSymbol = null,
            CreatedAt = now,
            UpdatedAt = now
        };

        var player = new GamePlayer
        {
            DisplayName = displayName.Trim(),
            Symbol = PlayerSymbol.X,
            IsConnected = false,
            WantsRematch = false,
            JoinedAt = now
        };

        room.Players.Add(player);
        _db.GameRooms.Add(room);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return RoomActionResult.Fail("Could not create a room right now. Please try again.");
        }

        return RoomActionResult.Ok(room.Id, room.RoomCode, player.Id, player.Symbol);
    }

    public async Task<RoomActionResult> JoinRoomAsync(
        string roomCode,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        string? nameError = ValidateDisplayName(displayName);
        if (nameError is not null)
        {
            return RoomActionResult.Fail(nameError);
        }

        string? codeError = ValidateRoomCode(roomCode, out string normalizedCode);
        if (codeError is not null)
        {
            return RoomActionResult.Fail(codeError);
        }

        GameRoom? room = await _db.GameRooms
            .Include(r => r.Players)
            .FirstOrDefaultAsync(r => r.RoomCode == normalizedCode, cancellationToken);

        if (room is null)
        {
            return RoomActionResult.Fail("Room not found.");
        }

        if (room.Players.Count >= 2)
        {
            return RoomActionResult.Fail("That room is full.");
        }

        if (room.Status != GameStatus.WaitingForPlayer)
        {
            return RoomActionResult.Fail("That room is not accepting players.");
        }

        PlayerSymbol symbol = room.Players.Any(p => p.Symbol == PlayerSymbol.X)
            ? PlayerSymbol.O
            : PlayerSymbol.X;

        DateTime now = DateTime.UtcNow;
        var player = new GamePlayer
        {
            DisplayName = displayName.Trim(),
            Symbol = symbol,
            IsConnected = false,
            WantsRematch = false,
            JoinedAt = now
        };

        player.Room = room;
        room.Players.Add(player);
        _db.GamePlayers.Add(player);
        room.Status = GameStatus.InProgress;
        room.CurrentTurn = PlayerSymbol.X;
        room.UpdatedAt = now;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return RoomActionResult.Fail("That room is already full.");
        }

        return RoomActionResult.Ok(room.Id, room.RoomCode, player.Id, player.Symbol);
    }

    public async Task<RoomInfo?> GetRoomAsync(
        string roomCode,
        CancellationToken cancellationToken = default)
    {
        string? codeError = ValidateRoomCode(roomCode, out string normalizedCode);
        if (codeError is not null)
        {
            return null;
        }

        GameRoom? room = await _db.GameRooms
            .AsNoTracking()
            .Include(r => r.Players)
            .FirstOrDefaultAsync(r => r.RoomCode == normalizedCode, cancellationToken);

        if (room is null)
        {
            return null;
        }

        return RoomInfo.FromRoom(room);
    }

    public async Task<RoomInfo?> SetPlayerConnectedAsync(
        string roomCode,
        Guid playerId,
        bool isConnected,
        CancellationToken cancellationToken = default)
    {
        string? codeError = ValidateRoomCode(roomCode, out string normalizedCode);
        if (codeError is not null)
        {
            return null;
        }

        GameRoom? room = await _db.GameRooms
            .Include(r => r.Players)
            .FirstOrDefaultAsync(r => r.RoomCode == normalizedCode, cancellationToken);

        if (room is null)
        {
            return null;
        }

        GamePlayer? player = room.Players.FirstOrDefault(p => p.Id == playerId);
        if (player is null)
        {
            return null;
        }

        if (player.IsConnected != isConnected)
        {
            player.IsConnected = isConnected;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
        }

        return RoomInfo.FromRoom(room);
    }

    private async Task<string?> GenerateUniqueRoomCodeAsync(CancellationToken cancellationToken)
    {
        for (int attempt = 0; attempt < MaxCodeGenerationAttempts; attempt++)
        {
            string code = GenerateRoomCode();
            bool exists = await _db.GameRooms.AnyAsync(r => r.RoomCode == code, cancellationToken);
            if (!exists)
            {
                return code;
            }
        }

        return null;
    }

    private static string GenerateRoomCode()
    {
        char[] code = new char[RoomCodeLength];
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = RoomCodeAlphabet[RandomNumberGenerator.GetInt32(RoomCodeAlphabet.Length)];
        }

        return new string(code);
    }

    private static string? ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return "Enter a display name.";
        }

        if (displayName.Trim().Length > MaxDisplayNameLength)
        {
            return "Display name must be 20 characters or fewer.";
        }

        return null;
    }

    private static string? ValidateRoomCode(string roomCode, out string normalizedCode)
    {
        normalizedCode = string.Empty;

        if (string.IsNullOrWhiteSpace(roomCode))
        {
            return "Enter a 6-character room code.";
        }

        normalizedCode = roomCode.Trim().ToUpperInvariant();
        if (normalizedCode.Length != RoomCodeLength)
        {
            return "Enter a 6-character room code.";
        }

        return null;
    }
}
