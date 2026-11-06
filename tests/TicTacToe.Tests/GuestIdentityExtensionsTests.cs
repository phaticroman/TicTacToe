using Microsoft.AspNetCore.Http;
using TicTacToe.Web.Services;

namespace TicTacToe.Tests;

public class GuestIdentityExtensionsTests
{
    [Fact]
    public void SetGuestIdentity_WritesHttpOnlyCookie()
    {
        var context = new DefaultHttpContext();
        Guid playerId = Guid.NewGuid();

        context.SetGuestIdentity("abc123", playerId);

        string setCookie = context.Response.Headers.SetCookie.ToString();
        Assert.Contains("tictactoe.guest=", setCookie);
        Assert.Contains("ABC123", setCookie);
        Assert.Contains(playerId.ToString(), setCookie);
        Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryGetGuestIdentity_ReadsCookie()
    {
        var context = new DefaultHttpContext();
        Guid playerId = Guid.NewGuid();
        context.Request.Headers.Cookie = $"tictactoe.guest=ABC123:{playerId}";

        bool found = context.TryGetGuestIdentity(out string roomCode, out Guid parsedPlayerId);

        Assert.True(found);
        Assert.Equal("ABC123", roomCode);
        Assert.Equal(playerId, parsedPlayerId);
    }

    [Fact]
    public void TryGetGuestIdentity_NormalizesRoomCode()
    {
        var context = new DefaultHttpContext();
        Guid playerId = Guid.NewGuid();
        context.Request.Headers.Cookie = $"tictactoe.guest=abc123:{playerId}";

        bool found = context.TryGetGuestIdentity(out string roomCode, out Guid parsedPlayerId);

        Assert.True(found);
        Assert.Equal("ABC123", roomCode);
        Assert.Equal(playerId, parsedPlayerId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("nope")]
    [InlineData(":00000000-0000-0000-0000-000000000000")]
    [InlineData("ABC123:")]
    [InlineData("ABC123:not-a-guid")]
    public void TryGetGuestIdentity_RejectsMalformedCookie(string cookieValue)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = $"tictactoe.guest={cookieValue}";

        bool found = context.TryGetGuestIdentity(out string roomCode, out Guid playerId);

        Assert.False(found);
        Assert.Equal(string.Empty, roomCode);
        Assert.Equal(Guid.Empty, playerId);
    }

    [Fact]
    public void ClearGuestIdentity_ExpiresCookie()
    {
        var context = new DefaultHttpContext();

        context.ClearGuestIdentity();

        string setCookie = context.Response.Headers.SetCookie.ToString();
        Assert.Contains("tictactoe.guest=", setCookie);
        Assert.Contains("expires=", setCookie, StringComparison.OrdinalIgnoreCase);
    }
}
