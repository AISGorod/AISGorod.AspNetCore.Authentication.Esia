namespace AISGorod.AspNetCore.Authentication.Esia.BouncyCastle.Options;

/// <summary>
/// Интерфейс для предоставления настроек Bouncy Castle.
/// </summary>
public interface IBouncyCastleOptions
{
    /// <summary>
    /// Путь к файлу приватного ключа.
    /// </summary>
    /// <remarks>
    /// Поле обязательное.
    /// </remarks>
    string? KeyFilePath { get; }

    /// <summary>
    /// Путь к файлу сертификата.
    /// </summary>
    /// <remarks>
    /// Поле обязательное.
    /// </remarks>
    string? CertFilePath { get; }
}