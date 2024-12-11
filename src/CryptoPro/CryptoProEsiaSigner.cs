using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using AISGorod.AspNetCore.Authentication.Esia.CryptoPro.Options;
using CryptoPro.Security.Cryptography;
using CryptoPro.Security.Cryptography.Pkcs;
using CryptoPro.Security.Cryptography.X509Certificates;

namespace AISGorod.AspNetCore.Authentication.Esia.CryptoPro;

/// <summary>
/// Простейшая обёртка подписи запросов над КриптоПро.
/// </summary>
public class CryptoProEsiaSigner(ICryptoProOptions options) : IEsiaSigner
{
    /// <inheritdoc />
    public string Sign(byte[] data)
    {
        ValidateOptions();

        using var gostCert = FindCertificateByThumbprint(options.CertThumbprint!);
        using var signingKey = GetSigningKey(gostCert);

        ConfigureSigningKeyPassword(signingKey, options.CertPin!);

        var signature = CreateSignature(data, gostCert, signingKey);

        return Convert.ToBase64String(signature);
    }

    /// <summary>
    /// Проверяет корректность настроек.
    /// </summary>
    /// <exception cref="InvalidOperationException">Выбрасывается, если настройки некорректны.</exception>
    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(options.CertThumbprint) || string.IsNullOrWhiteSpace(options.CertPin))
        {
            throw new InvalidOperationException(
                $"Для подключения КриптоПро необходимо указать {nameof(options.CertThumbprint)} и {nameof(options.CertPin)}.");
        }
    }

    /// <summary>
    /// Выполняет поиск сертификата по отпечатку.
    /// </summary>
    /// <param name="certThumbprint">Отпечаток сертификата.</param>
    /// <returns>Найденный сертификат.</returns>
    /// <exception cref="CryptographicException">Выбрасывается, если сертификат не найден.</exception>
    private static CpX509Certificate2 FindCertificateByThumbprint(string certThumbprint)
    {
        return FindInStore(StoreLocation.LocalMachine) ?? FindInStore(StoreLocation.CurrentUser)
            ?? throw new CryptographicException("Сертификат с указанным отпечатком не найден.");

        CpX509Certificate2? FindInStore(StoreLocation location)
        {
            using var store = new CpX509Store(StoreName.My, location);
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);
            return certs.Count == 1 ? certs[0] : null;
        }
    }

    /// <summary>
    /// Получает ключ для подписи из сертификата.
    /// </summary>
    /// <param name="cert">Сертификат.</param>
    /// <returns>Ключ для подписи.</returns>
    private static Gost3410_2012_256CryptoServiceProvider GetSigningKey(CpX509Certificate2 cert)
    {
        if (cert.GetGost3410_2012_256PrivateKey() is not Gost3410_2012_256CryptoServiceProvider signingKey)
        {
            throw new CryptographicException("Ключ подписи недоступен или несовместим.");
        }

        return signingKey;
    }

    /// <summary>
    /// Устанавливает пароль для контейнера ключа.
    /// </summary>
    /// <param name="signingKey">Ключ для подписи.</param>
    /// <param name="password">Пароль в виде строки.</param>
    private static void ConfigureSigningKeyPassword(Gost3410_2012_256CryptoServiceProvider? signingKey, string password)
    {
        if (signingKey == null || string.IsNullOrEmpty(password)) return;

        var securePassword = new SecureString();
        foreach (var ch in password)
        {
            securePassword.AppendChar(ch);
        }

        signingKey.SetContainerPassword(securePassword);
    }

    /// <summary>
    /// Создаёт подпись данных.
    /// </summary>
    /// <param name="data">Данные для подписи.</param>
    /// <param name="cert">Сертификат.</param>
    /// <param name="signingKey">Ключ для подписи.</param>
    /// <returns>Подпись в виде массива байтов.</returns>
    private static byte[] CreateSignature(byte[] data, CpX509Certificate2 cert, Gost3410_2012_256CryptoServiceProvider signingKey)
    {
        var contentInfo = new ContentInfo(data);
        var signedCms = new CpSignedCms(contentInfo, true);
        var cmsSigner = new CpCmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, cert, signingKey);

        cmsSigner.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.Now));
        cmsSigner.SignedAttributes.Add(new PkcsSigningCertificateV2(cert));

        signedCms.ComputeSignature(cmsSigner);

        return signedCms.Encode();
    }
}