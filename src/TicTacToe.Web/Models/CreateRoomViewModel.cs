using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Web.Models;

public class CreateRoomViewModel
{
    [Required(ErrorMessage = "Enter a display name.")]
    [StringLength(20, ErrorMessage = "Display name must be 20 characters or fewer.")]
    [Display(Name = "Display name")]
    public string DisplayName { get; set; } = string.Empty;
}
