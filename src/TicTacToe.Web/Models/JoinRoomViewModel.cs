using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Web.Models;

public class JoinRoomViewModel
{
    [Required(ErrorMessage = "Enter a 6-character room code.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Enter a 6-character room code.")]
    [Display(Name = "Room code")]
    public string RoomCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter a display name.")]
    [StringLength(20, ErrorMessage = "Display name must be 20 characters or fewer.")]
    [Display(Name = "Display name")]
    public string DisplayName { get; set; } = string.Empty;
}
