using System.ComponentModel.DataAnnotations;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;

public class EditManagerRequest
{
    [MinLength(1)]
    public string? Name { get; set; }

    [MinLength(1)]
    public string? Surname { get; set; }

    [MinLength(1)]
    public string? LastName { get; set; }

    [RegularExpression(@"^\+7(\d){10}$", ErrorMessage = "Телефон должен быть в формате +7XXXXXXXXXX")]
    public string? Phone { get; set; }

    public DateTime? Birthday { get; set; }
    public Gender? Gender { get; set; }

    [MinLength(1)]
    public string? Nationality { get; set; }

    public Role? Role { get; set; }
}
