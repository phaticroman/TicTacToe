using Microsoft.AspNetCore.Mvc;
using TicTacToe.Web.Models;
using TicTacToe.Web.Services;

namespace TicTacToe.Web.Controllers;

public class GameController : Controller
{
    private readonly RoomService _roomService;

    public GameController(RoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task<IActionResult> Index(string? code, CancellationToken cancellationToken)
    {
        if (!HttpContext.TryGetGuestIdentity(out string cookieCode, out Guid playerId))
        {
            return RedirectHome();
        }

        if (!string.IsNullOrWhiteSpace(code) &&
            !string.Equals(code.Trim(), cookieCode, StringComparison.OrdinalIgnoreCase))
        {
            return RedirectHome();
        }

        RoomInfo? room = await _roomService.GetRoomAsync(cookieCode, cancellationToken);
        if (room is null)
        {
            return RedirectHome();
        }

        RoomPlayerInfo? currentPlayer = room.Players.FirstOrDefault(p => p.PlayerId == playerId);
        if (currentPlayer is null)
        {
            return RedirectHome();
        }

        return View(new GameViewModel
        {
            Room = room,
            CurrentPlayer = currentPlayer
        });
    }

    private IActionResult RedirectHome()
    {
        HttpContext.ClearGuestIdentity();
        return RedirectToAction("Index", "Home");
    }
}
