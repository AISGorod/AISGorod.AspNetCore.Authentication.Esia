namespace EsiaNet8Sample.Models;

/// <summary>
/// Вью модель выбора организации.
/// </summary>
public class OrganizationSelectViewModel
{
    /// <summary>
    /// Роли.
    /// </summary>
    public List<EsiaPersonRoles> PersonRoles { get; init; } = [];
}