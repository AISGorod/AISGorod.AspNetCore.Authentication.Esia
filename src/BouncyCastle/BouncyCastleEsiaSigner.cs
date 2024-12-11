using System;
using System.IO;
using AISGorod.AspNetCore.Authentication.Esia.BouncyCastle.Options;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Rosstandart;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
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
    public string Sign(byte[] data)
    {
        ValidateOptions();

        var cert = LoadCertificate(options.CertFilePath!);
        var key = LoadPrivateKey(options.KeyFilePath!);

        var signedData = CreateSignedDataGenerator(cert, key);

        using var memoryStream = new MemoryStream();
        using (var signedStream = signedData.Open(memoryStream, true))
        {
            signedStream.Write(data, 0, data.Length);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
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
    /// Создание генератора подписанных данных на основе сертификата и приватного ключа.
    /// </summary>
    /// <param name="cert">Сертификат X509.</param>
    /// <param name="key">Приватный ключ.</param>
    /// <returns>Экземпляр <see cref="CmsSignedDataStreamGenerator"/> для подписания данных.</returns>
    /// <exception cref="NotSupportedException">Используется неподдерживаемый тип хеширования.</exception>
    private static CmsSignedDataStreamGenerator CreateSignedDataGenerator(X509Certificate cert, AsymmetricKeyParameter key)
    {
        var digestOid = GetDigestOid(cert.SigAlgName);

        var generator = new CmsSignedDataStreamGenerator();
        generator.AddSigner(key, cert, digestOid);
        generator.AddCertificate(cert);

        return generator;
    }

    /// <summary>
    /// Получает идентификатор OID для типа хеширования на основе имени алгоритма подписи.
    /// </summary>
    /// <param name="algorithm">Имя алгоритма подписи.</param>
    /// <returns>Идентификатор OID для хеширования.</returns>
    /// <exception cref="NotSupportedException">Алгоритм подписи не поддерживается.</exception>
    private static string GetDigestOid(string algorithm)
    {
        return algorithm switch
        {
            GOST_2012_256 => RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256.Id,
            GOST_2012_512 => RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512.Id,
            _ => throw new NotSupportedException("Не поддерживаемый тип хеширования."),
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
            throw new FileNotFoundException($"Файл сертификата не найден: {certPath}");

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
            throw new FileNotFoundException($"Файл ключа не найден: {keyPath}");

        using var reader = File.OpenText(keyPath);
        var pemReader = new Org.BouncyCastle.Utilities.IO.Pem.PemReader(reader);
        var pemObject = pemReader.ReadPemObject();
        var privateKeyInfo = PrivateKeyInfo.GetInstance(pemObject.Content);

        return PrivateKeyFactory.CreateKey(privateKeyInfo);
    }
}