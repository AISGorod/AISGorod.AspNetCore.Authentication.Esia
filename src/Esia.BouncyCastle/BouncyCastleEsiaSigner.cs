using System;
using System.IO;
using System.Text;
using AISGorod.AspNetCore.Authentication.Esia.BouncyCastle.Options;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace AISGorod.AspNetCore.Authentication.Esia.BouncyCastle;

/// <summary>
/// Настройка подписи через bouncy castle.
/// </summary>
public class BouncyCastleEsiaSigner(IBouncyCastleOptions options) : IEsiaSigner
{
    /// <summary>
    /// Тип хеширования гост 2012 256б.
    /// </summary>
    private const string GOST_2012_256 = "ECGOST3410-2012-256";

    /// <summary>
    /// Тип хеширования гост 2012 512б.
    /// </summary>
    private const string GOST_2012_512 = "ECGOST3410-2012-512";

    /// <inheritdoc />
    public string Sign(string concatenatedString)
    {
        ValidateOptions();

        var signData = Encoding.UTF8.GetBytes(concatenatedString);
        var privateKey = LoadPrivateKey(options.KeyFilePath!);

        var signer = SignerUtilities.GetSigner(GOST_2012_256);
        signer.Init(true, privateKey);
        signer.BlockUpdate(signData, 0, signData.Length);

        // Создание подписи
        var signature = signer.GenerateSignature();

        return Convert.ToBase64String(signature);
    }

    /// <inheritdoc />
    public string GetCertificateFingerprint()
    {
        ValidateOptions();

        var cert = LoadCertificate(options.CertFilePath!);
        var digest = GetDigest(cert.SigAlgName);

        var certBytes = cert.GetEncoded();

        var hash = ComputeHash(digest, certBytes);
        return Convert.ToHexString(hash).ToUpperInvariant();
    }

    /// <summary>
    /// Вычисление хеша.
    /// </summary>
    /// <param name="digest">Тип хеша.</param>
    /// <param name="data">Массив байтов.</param>
    /// <returns>Хеш.</returns>
    private static byte[] ComputeHash(IDigest digest, byte[] data)
    {
        digest.BlockUpdate(data, 0, data.Length);
        var result = new byte[digest.GetDigestSize()];
        digest.DoFinal(result, 0);
        return result;
    }

    /// <summary>
    /// Валидация настроек.
    /// </summary>
    /// <exception cref="InvalidOperationException">Не указаны пути до ключа и сертификата.</exception>
    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(options.CertFilePath) || string.IsNullOrWhiteSpace(options.KeyFilePath))
        {
            throw new InvalidOperationException($"Для подключения Bouncy Castle необходимо указать {nameof(options.KeyFilePath)} и {nameof(options.CertFilePath)}.");
        }
    }

    /// <summary>
    /// Получает тип хеширования на основе имени алгоритма подписи.
    /// </summary>
    /// <param name="algorithm">Имя алгоритма подписи.</param>
    /// <returns>Идентификатор OID для хеширования.</returns>
    /// <exception cref="NotSupportedException">Алгоритм подписи не поддерживается.</exception>
    private static IDigest GetDigest(string algorithm)
    {
        return algorithm switch
        {
            GOST_2012_256 => new Gost3411_2012_256Digest(),
            GOST_2012_512 => new Gost3411_2012_512Digest(),
            _ => new Gost3411_2012_256Digest()
        };
    }

    /// <summary>
    /// Загружает сертификат из файла.
    /// </summary>
    /// <param name="certPath">Путь к файлу сертификата.</param>
    /// <returns>Сертификат X509.</returns>
    /// <exception cref="FileNotFoundException">Файл сертификата не найден.</exception>
    private static X509Certificate LoadCertificate(string certPath)
    {
        if (!File.Exists(certPath))
        {
            throw new FileNotFoundException($"Файл сертификата не найден: {certPath}");
        }

        var parser = new X509CertificateParser();
        return parser.ReadCertificate(File.ReadAllBytes(certPath));
    }

    /// <summary>
    /// Загружает приватный ключ из PEM-файла.
    /// </summary>
    /// <param name="keyPath">Путь к файлу ключа.</param>
    /// <returns>Приватный ключ.</returns>
    /// <exception cref="FileNotFoundException">Файл ключа не найден.</exception>
    private static AsymmetricKeyParameter LoadPrivateKey(string keyPath)
    {
        if (!File.Exists(keyPath))
        {
            throw new FileNotFoundException($"Файл ключа не найден: {keyPath}");
        }

        using var reader = File.OpenText(keyPath);
        var pemReader = new Org.BouncyCastle.Utilities.IO.Pem.PemReader(reader);
        var pemObject = pemReader.ReadPemObject();
        var privateKeyInfo = PrivateKeyInfo.GetInstance(pemObject.Content);

        return PrivateKeyFactory.CreateKey(privateKeyInfo);
    }
}