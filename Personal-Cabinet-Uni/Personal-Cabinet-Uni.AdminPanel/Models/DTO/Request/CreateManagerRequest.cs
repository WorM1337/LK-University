using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;

public class CreateManagerRequest
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public Role Role { get; set; } = Role.Manager;
}
