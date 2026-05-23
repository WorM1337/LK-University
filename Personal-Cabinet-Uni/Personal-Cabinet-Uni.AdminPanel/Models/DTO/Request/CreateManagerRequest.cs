using System.ComponentModel.DataAnnotations;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;

public class CreateManagerRequest
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Surname { get; set; } = string.Empty;

    [MinLength(1)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [RegularExpression(@"^\+7(\d){10}$", ErrorMessage = "Телефон должен быть в формате +7XXXXXXXXXX")]
    public string Phone { get; set; } = string.Empty;

    public DateTime? Birthday { get; set; }

    public Gender? Gender { get; set; }

    [Required]
    [MinLength(1)]
    public string Nationality { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } = Role.Manager;
}
