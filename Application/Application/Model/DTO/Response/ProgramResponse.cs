using Application.Model.Enums;

namespace Application.Model.DTO.Response;

/// <summary>
/// Программа обучения
/// </summary>
public class ProgramResponse
{
    public Guid Id { get; set; }
    /// <summary>
    /// Название программы
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Код специальности программы (xx.xx.xx)
    /// </summary>
    public string SpecialityCode { get; set; }
    /// <summary>
    /// Основной язык обучения
    /// </summary>
    public string StudyingLanguage { get; set; }
    /// <summary>
    /// Форма обучения
    /// </summary>
    public FormOfEducation FormOfEducation { get; set; }
    /// <summary>
    /// Название факультета
    /// </summary>
    public string Faculty { get; set; }
    /// <summary>
    /// Уровень образования
    /// </summary>
    public string EducationLevel { get; set; }
}