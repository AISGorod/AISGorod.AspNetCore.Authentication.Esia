namespace AISGorod.AspNetCore.Authentication.Esia.Options;

/// <summary>
/// Настройки bouncy castle.
/// </summary>
public class BouncyCastleOptions
{
    /// <summary>
    /// Ключ.
    /// </summary>
    public string? KeyFilePath { get; set; }

    /// <summary>
    /// Сертификат.
    /// </summary>
    public string? CertFilePath { get; set; }
}