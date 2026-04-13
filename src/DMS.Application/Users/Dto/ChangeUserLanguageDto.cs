using System.ComponentModel.DataAnnotations;

namespace DMS.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}