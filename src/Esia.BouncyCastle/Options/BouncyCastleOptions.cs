namespace AISGorod.AspNetCore.Authentication.Esia.BouncyCastle.Options;

/// <summary>
/// Настройки bouncy castle.
/// </summary>
public class BouncyCastleOptions : IBouncyCastleOptions
{
    /// <inheritdoc />
    public string? KeyFilePath { get; set; }

    /// <inheritdoc />
    public string? CertFilePath { get; set; }
}