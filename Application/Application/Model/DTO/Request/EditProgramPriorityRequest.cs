using System.ComponentModel.DataAnnotations;

namespace Application.Model.DTO.Request;

/// <summary>
/// Запрос на изменение приоритета программы в заявке абитуриента
/// </summary>
public class EditProgramPriorityRequest
{
    /// <summary>
    /// ID программы внутри заявки
    /// </summary>
    [Required]
    public Guid ProgramId { get; set; }
    /// <summary>
    /// Приоритет программы в заявке. Может принимать значения от 1 до N, где N - макс. количество программ в заявке
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Priority { get; set; }
}