namespace TicTacToe.Web.Models;

public class HomeViewModel
{
    public CreateRoomViewModel Create { get; set; } = new();
    public JoinRoomViewModel Join { get; set; } = new();
}
