using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TicTacToe.Web.Services;

namespace TicTacToe.Web.Hubs;

public class GameHub : Hub
{
    private const string RoomCodeKey = "tictactoe.roomCode";
    private const string PlayerIdKey = "tictactoe.playerId";
    private const string JoinedKey = "tictactoe.joined";

    private readonly RoomService _roomService;
    private readonly GameService _gameService;
    private readonly PlayerConnectionTracker _connections;
    private readonly ILogger<GameHub> _logger;

    public GameHub(
        RoomService roomService,
        GameService gameService,
        PlayerConnectionTracker connections,
        ILogger<GameHub> logger)
    {
        _roomService = roomService;
        _gameService = gameService;
        _connections = connections;
        _logger = logger;
    }

    public static string GroupName(string roomCode) =>
        $"room:{roomCode.Trim().ToUpperInvariant()}";

    public async Task JoinRoom(string? roomCode)
    {
        CancellationToken cancellationToken = Context.ConnectionAborted;
        HttpContext? httpContext = Context.GetHttpContext();
        if (httpContext is null ||
            !httpContext.TryGetGuestIdentity(out string cookieCode, out Guid playerId))
        {
            throw new HubException("Your room session has expired. Return home and join again.");
        }

        if (string.IsNullOrWhiteSpace(roomCode) ||
            !string.Equals(roomCode.Trim(), cookieCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new HubException("You are not part of this room.");
        }

        RoomInfo? room = await _roomService.GetRoomAsync(cookieCode, cancellationToken);
        if (room is null || room.Players.All(p => p.PlayerId != playerId))
        {
            throw new HubException("You are not part of this room.");
        }

        Context.Items[RoomCodeKey] = cookieCode;
        Context.Items[PlayerIdKey] = playerId;
        Context.Items[JoinedKey] = true;

        _connections.Connect(Context.ConnectionId, cookieCode, playerId);
        RoomInfo? connectedRoom = await _roomService.SetPlayerConnectedAsync(
            cookieCode,
            playerId,
            true,
            cancellationToken);
        if (connectedRoom is not null)
        {
            room = connectedRoom;
        }

        string groupName = GroupName(cookieCode);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName, cancellationToken);
        await Clients.Group(groupName).SendAsync("RoomState", room, cancellationToken);
    }

    public async Task MakeMove(int cellIndex)
    {
        CancellationToken cancellationToken = Context.ConnectionAborted;
        if (!TryGetJoinedContext(out string roomCode, out Guid playerId))
        {
            throw new HubException("Join the room before making a move.");
        }

        GameActionResult result = await _gameService.MakeMoveAsync(
            roomCode,
            playerId,
            cellIndex,
            cancellationToken);

        if (!result.Success)
        {
            await Clients.Caller.SendAsync("MoveRejected", result.Error, cancellationToken);
            return;
        }

        await Clients.Group(GroupName(roomCode)).SendAsync("RoomState", result.Room, cancellationToken);
    }

    public async Task RequestRematch()
    {
        CancellationToken cancellationToken = Context.ConnectionAborted;
        if (!TryGetJoinedContext(out string roomCode, out Guid playerId))
        {
            throw new HubException("Join the room before requesting a rematch.");
        }

        GameActionResult result = await _gameService.RequestRematchAsync(
            roomCode,
            playerId,
            cancellationToken);

        if (!result.Success)
        {
            await Clients.Caller.SendAsync("RematchRejected", result.Error, cancellationToken);
            return;
        }

        await Clients.Group(GroupName(roomCode)).SendAsync("RoomState", result.Room, cancellationToken);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        PlayerConnectionTracker.PlayerConnection? disconnected = _connections.Disconnect(Context.ConnectionId);
        if (disconnected is not null)
        {
            try
            {
                RoomInfo? room = await _roomService.SetPlayerConnectedAsync(
                    disconnected.RoomCode,
                    disconnected.PlayerId,
                    false,
                    CancellationToken.None);

                if (room is not null)
                {
                    await Clients.Group(GroupName(disconnected.RoomCode))
                        .SendAsync("RoomState", room, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not update presence for player {PlayerId}.", disconnected.PlayerId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private bool TryGetJoinedContext(out string roomCode, out Guid playerId)
    {
        roomCode = string.Empty;
        playerId = Guid.Empty;

        if (!Context.Items.TryGetValue(JoinedKey, out object? joined) || joined is not true ||
            !Context.Items.TryGetValue(RoomCodeKey, out object? roomCodeValue) || roomCodeValue is not string code ||
            !Context.Items.TryGetValue(PlayerIdKey, out object? playerIdValue) || playerIdValue is not Guid id)
        {
            return false;
        }

        roomCode = code;
        playerId = id;
        return true;
    }
}
