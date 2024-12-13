using System.Text.Json.Serialization;

namespace EsiaNet8Sample.Models;

/// <summary>
/// Пользователь.
/// </summary>
public class EsiaPersonRoles
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    [JsonPropertyName("oid")]
    public int Id { get; init; }

    /// <summary>
    /// Полное имя.
    /// </summary>
    [JsonPropertyName("fullName")]
    public string? FullName { get; init; }

    /// <summary>
    /// Короткое имя.
    /// </summary>
    [JsonPropertyName("shortName")]
    public string? ShortName { get; init; }

    /// <summary>
    /// Огрн.
    /// </summary>
    [JsonPropertyName("ogrn")]
    public string? Ogrn { get; init; }

    // Тут должно быть намного больше свойств. Но текущего набора достаточно.
}