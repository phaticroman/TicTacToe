using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Controllers;
using TicTacToe.Web.Data;
using TicTacToe.Web.Models;
using TicTacToe.Web.Services;

namespace TicTacToe.Tests;

public class HomeControllerTests
{
    private static AppDbContext CreateContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static HomeController CreateController(AppDbContext db)
    {
        return new HomeController(new RoomService(db))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Create_WithValidName_RedirectsToGameAndSetsCookie()
    {
        using AppDbContext db = CreateContext();
        HomeController controller = CreateController(db);

        IActionResult result = await controller.Create(
            new CreateRoomViewModel { DisplayName = "Alice" },
            CancellationToken.None);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Game", redirect.ControllerName);
        string code = Assert.IsType<string>(redirect.RouteValues!["code"]);
        Assert.Equal(6, code.Length);

        string setCookie = controller.HttpContext.Response.Headers.SetCookie.ToString();
        Assert.Contains("tictactoe.guest=", setCookie);
        Assert.Contains(code, setCookie);
    }

    [Fact]
    public async Task Create_WithInvalidName_ReturnsHomeViewWithError()
    {
        using AppDbContext db = CreateContext();
        HomeController controller = CreateController(db);

        IActionResult result = await controller.Create(
            new CreateRoomViewModel { DisplayName = "   " },
            CancellationToken.None);

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
        Assert.IsType<HomeViewModel>(view.Model);
        Assert.Contains(
            controller.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage == "Enter a display name.");
        Assert.Empty(db.GameRooms);
    }

    [Fact]
    public async Task Join_WithValidRoom_RedirectsToGameAndSetsCookie()
    {
        using AppDbContext db = CreateContext();
        var service = new RoomService(db);
        RoomActionResult created = await service.CreateRoomAsync("Alice");
        HomeController controller = CreateController(db);

        IActionResult result = await controller.Join(
            new JoinRoomViewModel
            {
                RoomCode = created.RoomCode!.ToLowerInvariant(),
                DisplayName = "Bob"
            },
            CancellationToken.None);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Game", redirect.ControllerName);
        Assert.Equal(created.RoomCode, redirect.RouteValues!["code"]);

        string setCookie = controller.HttpContext.Response.Headers.SetCookie.ToString();
        Assert.Contains("tictactoe.guest=", setCookie);
        Assert.Contains(created.RoomCode!, setCookie);
    }

    [Fact]
    public async Task Join_WithUnknownRoom_ReturnsHomeViewWithError()
    {
        using AppDbContext db = CreateContext();
        HomeController controller = CreateController(db);

        IActionResult result = await controller.Join(
            new JoinRoomViewModel { RoomCode = "ZZZZZZ", DisplayName = "Bob" },
            CancellationToken.None);

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", view.ViewName);
        Assert.IsType<HomeViewModel>(view.Model);
        Assert.Contains(
            controller.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage == "Room not found.");
    }
}
