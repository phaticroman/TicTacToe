using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Web.Models;
using TicTacToe.Web.Services;

namespace TicTacToe.Web.Controllers;

public class HomeController : Controller
{
    private readonly RoomService _roomService;

    public HomeController(RoomService roomService)
    {
        _roomService = roomService;
    }

    public IActionResult Index()
    {
        return View(new HomeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(Prefix = "Create")] CreateRoomViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", BuildHomeViewModel(model, null));
        }

        RoomActionResult result = await _roomService.CreateRoomAsync(model.DisplayName, cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View("Index", BuildHomeViewModel(model, null));
        }

        HttpContext.SetGuestIdentity(result.RoomCode!, result.PlayerId!.Value);
        return RedirectToAction("Index", "Game", new { code = result.RoomCode });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(
        [Bind(Prefix = "Join")] JoinRoomViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", BuildHomeViewModel(null, model));
        }

        RoomActionResult result = await _roomService.JoinRoomAsync(
            model.RoomCode,
            model.DisplayName,
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View("Index", BuildHomeViewModel(null, model));
        }

        HttpContext.SetGuestIdentity(result.RoomCode!, result.PlayerId!.Value);
        return RedirectToAction("Index", "Game", new { code = result.RoomCode });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static HomeViewModel BuildHomeViewModel(
        CreateRoomViewModel? create,
        JoinRoomViewModel? join)
    {
        return new HomeViewModel
        {
            Create = create ?? new CreateRoomViewModel(),
            Join = join ?? new JoinRoomViewModel()
        };
    }
}
