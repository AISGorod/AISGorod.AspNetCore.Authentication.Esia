namespace AISGorod.AspNetCore.Authentication.Esia.Options;

/// <summary>
/// Настройки crypto pro.
/// </summary>
public class CryptoProOptions
{
    /// <summary>
    /// Отпечаток сертификата.
    /// </summary>
    public string? CertThumbprint { get; set; }

    /// <summary>
    /// Pin сертификата.
    /// </summary>
    public string? CertPin { get; set; }
}