using Microsoft.AspNetCore.Mvc;
using TicTacToe.Web.Game;
using TicTacToe.Web.Models;
using TicTacToe.Web.Services;

namespace TicTacToe.Web.Controllers;

public class LocalGameController : Controller
{
    private const string SessionKey = "TicTacToe.LocalGame";

    private readonly LocalGameService _localGameService;

    public LocalGameController(LocalGameService localGameService)
    {
        _localGameService = localGameService;
    }

    public IActionResult Index()
    {
        LocalGameState state = GetState() ?? CreateState();
        SaveState(state);
        return View(LocalGameViewModel.FromState(state));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Move(int cellIndex)
    {
        LocalGameState? state = GetState();
        if (state is null)
        {
            return BadRequest(new { error = "This local game expired. Refresh the page to start a new one." });
        }

        MoveResult move = _localGameService.ApplyMove(state, cellIndex);
        if (!move.IsValid)
        {
            return BadRequest(new { error = move.Error });
        }

        SaveState(state);
        return Json(LocalGameViewModel.FromState(state));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NewRound()
    {
        LocalGameState? state = GetState();
        if (state is null)
        {
            return BadRequest(new { error = "This local game expired. Refresh the page to start a new one." });
        }

        _localGameService.StartNewRound(state);
        SaveState(state);
        return Json(LocalGameViewModel.FromState(state));
    }

    private LocalGameState? GetState() =>
        HttpContext.Session.GetObject<LocalGameState>(SessionKey);

    private void SaveState(LocalGameState state) =>
        HttpContext.Session.SetObject(SessionKey, state);

    private LocalGameState CreateState() => new();
}
