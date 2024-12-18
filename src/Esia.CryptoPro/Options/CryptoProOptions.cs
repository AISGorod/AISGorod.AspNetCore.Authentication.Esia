namespace AISGorod.AspNetCore.Authentication.Esia.CryptoPro.Options;

/// <summary>
/// Настройки crypto pro.
/// </summary>
public class CryptoProOptions : ICryptoProOptions
{
    /// <inheritdoc />
    public string? CertThumbprint { get; set; }

    /// <inheritdoc />
    public string? CertPin { get; set; }
}