namespace AISGorod.AspNetCore.Authentication.Esia.CryptoPro.Options;

/// <summary>
/// Интерфейс для предоставления настроек CryptoPro.
/// </summary>
public interface ICryptoProOptions
{
    /// <summary>
    /// Отпечаток сертификата.
    /// </summary>
    /// <remarks>
    /// Поле обязательное.
    /// </remarks>
    string? CertThumbprint { get; }

    /// <summary>
    /// Pin сертификата.
    /// </summary>
    string? CertPin { get; }
}