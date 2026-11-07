using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Web.Controllers;
using TicTacToe.Web.Enums;
using TicTacToe.Web.Models;
using TicTacToe.Web.Services;

namespace TicTacToe.Tests;

public class LocalGameControllerTests
{
    private const string SessionKey = "TicTacToe.LocalGame";

    [Fact]
    public void Index_CreatesFreshGameInSession()
    {
        DefaultHttpContext context = CreateHttpContext();
        LocalGameController controller = CreateController(context);

        IActionResult result = controller.Index();

        ViewResult view = Assert.IsType<ViewResult>(result);
        LocalGameViewModel model = Assert.IsType<LocalGameViewModel>(view.Model);
        Assert.Equal(GameStatus.InProgress, model.Status);
        Assert.Equal(PlayerSymbol.X, model.CurrentTurn);
        Assert.Equal("Player X's turn", model.StatusText);

        LocalGameState? state = context.Session.GetObject<LocalGameState>(SessionKey);
        Assert.NotNull(state);
        Assert.Equal(PlayerSymbol.X, state.CurrentTurn);
    }

    [Fact]
    public void Move_ValidMove_ReturnsUpdatedStateAndSavesSession()
    {
        DefaultHttpContext context = CreateHttpContext();
        context.Session.SetObject(SessionKey, new LocalGameState());
        LocalGameController controller = CreateController(context);

        IActionResult result = controller.Move(4);

        JsonResult json = Assert.IsType<JsonResult>(result);
        LocalGameViewModel model = Assert.IsType<LocalGameViewModel>(json.Value);
        Assert.Equal("----X----", model.Board);
        Assert.Equal(PlayerSymbol.O, model.CurrentTurn);
        Assert.Equal("Player O's turn", model.StatusText);

        LocalGameState? state = context.Session.GetObject<LocalGameState>(SessionKey);
        Assert.NotNull(state);
        Assert.Equal("----X----", state.Board);
    }

    [Fact]
    public void Move_OccupiedCell_ReturnsBadRequest()
    {
        DefaultHttpContext context = CreateHttpContext();
        context.Session.SetObject(SessionKey, new LocalGameState
        {
            Board = "----X----",
            CurrentTurn = PlayerSymbol.O
        });
        LocalGameController controller = CreateController(context);

        IActionResult result = controller.Move(4);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public void Move_WithoutSession_ReturnsBadRequest()
    {
        DefaultHttpContext context = CreateHttpContext();
        LocalGameController controller = CreateController(context);

        IActionResult result = controller.Move(0);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void NewRound_AfterWin_ResetsBoardAndKeepsScore()
    {
        DefaultHttpContext context = CreateHttpContext();
        context.Session.SetObject(SessionKey, new LocalGameState
        {
            Board = "XX-----O-",
            CurrentTurn = PlayerSymbol.X
        });
        LocalGameController controller = CreateController(context);
        controller.Move(2);

        IActionResult result = controller.NewRound();

        JsonResult json = Assert.IsType<JsonResult>(result);
        LocalGameViewModel model = Assert.IsType<LocalGameViewModel>(json.Value);
        Assert.Equal("---------", model.Board);
        Assert.Equal(GameStatus.InProgress, model.Status);
        Assert.Equal(PlayerSymbol.X, model.CurrentTurn);
        Assert.Equal(1, model.MatchesPlayed);
        Assert.Equal(1, model.XWins);
        Assert.False(model.IsGameOver);
    }

    [Fact]
    public void NewRound_WithoutSession_ReturnsBadRequest()
    {
        DefaultHttpContext context = CreateHttpContext();
        LocalGameController controller = CreateController(context);

        IActionResult result = controller.NewRound();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    private static LocalGameController CreateController(DefaultHttpContext context) =>
        new(new LocalGameService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = context
            }
        };

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Features.Set<ISessionFeature>(new TestSessionFeature());
        return context;
    }

    private sealed class TestSessionFeature : ISessionFeature
    {
        public ISession Session { get; set; } = new TestSession();
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _values = new();

        public bool IsAvailable => true;
        public string Id => "test-session";
        public IEnumerable<string> Keys => _values.Keys;

        public void Clear() => _values.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void Remove(string key) => _values.Remove(key);

        public void Set(string key, byte[] value) => _values[key] = value;

        public bool TryGetValue(string key, out byte[] value) =>
            _values.TryGetValue(key, out value!);
    }
}
