namespace AISGorod.AspNetCore.Authentication.Esia.CryptoPro;

/// <summary>
/// Идентификаторы объектов хеширования.
/// </summary>
public static class CryptoProOids
{
    /// <summary>
    /// ГОСТ Р 34.10-2012 (256 бит).
    /// </summary>
    public const string GOST3410_2012_256 = "1.2.643.7.1.1.1.1";

    /// <summary>
    /// ГОСТ Р 34.10-2012 (512 бит).
    /// </summary>
    public const string GOST3410_2012_512 = "1.2.643.7.1.1.1.2";

    /// <summary>
    /// ГОСТ Р 34.10-94.
    /// </summary>
    public const string GOST3410_94 = "1.2.643.2.2.19";

    /// <summary>
    /// SHA-1.
    /// </summary>
    public const string RSA_SHA1 = "1.2.840.113549.1.1.5";

    /// <summary>
    /// SHA-256.
    /// </summary>
    public const string RSA_SHA256 = "1.2.840.113549.1.1.11";
}