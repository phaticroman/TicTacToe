using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Controllers;
using TicTacToe.Web.Data;
using TicTacToe.Web.Models;
using TicTacToe.Web.Services;

namespace TicTacToe.Tests;

public class GameControllerTests
{
    private static AppDbContext CreateContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static GameController CreateController(AppDbContext db)
    {
        return new GameController(new RoomService(db))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Index_WithoutIdentity_RedirectsHome()
    {
        using AppDbContext db = CreateContext();
        GameController controller = CreateController(db);

        IActionResult result = await controller.Index(null, CancellationToken.None);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public async Task Index_WithValidIdentity_ReturnsRoomView()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");
        GameController controller = CreateController(db);
        controller.HttpContext.Request.Headers.Cookie =
            $"tictactoe.guest={created.RoomCode}:{created.PlayerId}";

        IActionResult result = await controller.Index(created.RoomCode, CancellationToken.None);

        ViewResult view = Assert.IsType<ViewResult>(result);
        GameViewModel model = Assert.IsType<GameViewModel>(view.Model);
        Assert.Equal(created.RoomCode, model.Room.RoomCode);
        Assert.Equal(created.PlayerId!.Value, model.CurrentPlayer.PlayerId);
        Assert.True(model.IsWaitingForOpponent);
        Assert.Equal("Waiting for opponent", model.StatusText);
    }

    [Fact]
    public async Task Index_WithMismatchedCode_ClearsIdentityAndRedirectsHome()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");
        GameController controller = CreateController(db);
        controller.HttpContext.Request.Headers.Cookie =
            $"tictactoe.guest={created.RoomCode}:{created.PlayerId}";

        IActionResult result = await controller.Index("ZZZZZZ", CancellationToken.None);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Home", redirect.ControllerName);
        string setCookie = controller.HttpContext.Response.Headers.SetCookie.ToString();
        Assert.Contains("expires=", setCookie, StringComparison.OrdinalIgnoreCase);
    }
}
