namespace Application.Model.DTO.Response;

/// <summary>
/// Программа с полем "Приоритет". Для вывода подробной информации о заявке и её программах
/// </summary>
public class ProgramResponseWithPriority : ProgramResponse
{
    /// <summary>
    /// Приоритет программы в заявке. Может принимать значения от 1 до N, где N - макс. количество программ в заявке
    /// </summary>
    public int Priority { get; set; }
}