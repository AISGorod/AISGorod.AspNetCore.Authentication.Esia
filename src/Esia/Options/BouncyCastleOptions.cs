namespace AISGorod.AspNetCore.Authentication.Esia.Options;

/// <summary>
/// Настройки bouncy castle.
/// </summary>
public class BouncyCastleOptions
{
    /// <summary>
    /// Ключ.
    /// </summary>
    /// <remarks>
    /// Поле обязательное.
    /// </remarks>
    public string? KeyFilePath { get; set; }

    /// <summary>
    /// Сертификат.
    /// </summary>
    /// <remarks>
    /// Поле обязательное.
    /// </remarks>
    public string? CertFilePath { get; set; }
}