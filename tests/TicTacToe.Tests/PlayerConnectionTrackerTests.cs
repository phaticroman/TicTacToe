using TicTacToe.Web.Services;

namespace TicTacToe.Tests;

public class PlayerConnectionTrackerTests
{
    [Fact]
    public void Connect_ReturnsTrueOnlyForFirstConnectionOfPlayer()
    {
        var tracker = new PlayerConnectionTracker();
        Guid playerId = Guid.NewGuid();

        Assert.True(tracker.Connect("one", "ABCDEF", playerId));
        Assert.False(tracker.Connect("two", "ABCDEF", playerId));
    }

    [Fact]
    public void Disconnect_ReturnsPlayerOnlyAfterLastConnectionCloses()
    {
        var tracker = new PlayerConnectionTracker();
        Guid playerId = Guid.NewGuid();
        tracker.Connect("one", "ABCDEF", playerId);
        tracker.Connect("two", "ABCDEF", playerId);

        Assert.Null(tracker.Disconnect("one"));

        PlayerConnectionTracker.PlayerConnection? connection = tracker.Disconnect("two");
        Assert.NotNull(connection);
        Assert.Equal("ABCDEF", connection!.RoomCode);
        Assert.Equal(playerId, connection.PlayerId);
    }

    [Fact]
    public void Disconnect_ReturnsNullForUnknownConnection()
    {
        var tracker = new PlayerConnectionTracker();

        Assert.Null(tracker.Disconnect("missing"));
    }

    [Fact]
    public void Connect_MovingConnectionToAnotherPlayerReleasesPreviousPlayer()
    {
        var tracker = new PlayerConnectionTracker();
        Guid firstPlayer = Guid.NewGuid();
        Guid secondPlayer = Guid.NewGuid();
        tracker.Connect("one", "ABCDEF", firstPlayer);

        Assert.True(tracker.Connect("one", "ABCDEF", secondPlayer));

        PlayerConnectionTracker.PlayerConnection? connection = tracker.Disconnect("one");
        Assert.NotNull(connection);
        Assert.Equal(secondPlayer, connection!.PlayerId);
    }
}
