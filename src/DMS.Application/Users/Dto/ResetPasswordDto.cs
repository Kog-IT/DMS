using System.ComponentModel.DataAnnotations;

namespace DMS.Users.Dto;

public class ResetPasswordDto
{
    [Required]
    public string AdminPassword { get; set; }

    [Required]
    public long UserId { get; set; }

    [Required]
    public string NewPassword { get; set; }
}
