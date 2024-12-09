namespace AISGorod.AspNetCore.Authentication.Esia.CryptoPro;

/// <summary>
/// Настройки crypto pro.
/// </summary>
public class CryptoProOptions
{
    /// <summary>
    /// Отпечаток сертификата.
    /// </summary>
    /// <remarks>
    /// Поле обязательное.
    /// </remarks>
    public string? CertThumbprint { get; set; }

    /// <summary>
    /// Pin сертификата.
    /// </summary>
    public string? CertPin { get; set; }
}