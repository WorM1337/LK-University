using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;

public class EditManagerRequest
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public DateTime? Birthday { get; set; }
    public Gender? Gender { get; set; }
    public string? Nationality { get; set; }
    public Role? Role { get; set; }
}
