using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using AISGorod.AspNetCore.Authentication.Esia.CryptoPro.Options;
using CryptoPro.Security.Cryptography;
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

        var signature = CreateSignature(data, signingKey);
        return Convert.ToBase64String(signature);
    }

    /// <inheritdoc />
    public string GetCertificateFingerprint()
    {
        ValidateOptions();

        using var gostCert = FindCertificateByThumbprint(options.CertThumbprint!);

        var keyAlgorithm = gostCert.GetKeyAlgorithm();
        var hashAlgorithmName = GetHashAlgorithmByKeyAlgorithm(keyAlgorithm);

        // Возвращаем отпечаток сертификата
        return gostCert.GetCertHashString(hashAlgorithmName);
    }
    
    /// <summary>
    /// Получение типа хеширования по ключу.
    /// </summary>
    /// <param name="keyAlgorithm"></param>
    /// <returns>Название алгоритма хеширования.</returns>
    private static CpHashAlgorithmName GetHashAlgorithmByKeyAlgorithm(string keyAlgorithm)
    {
        return keyAlgorithm switch
        {
            // ГОСТ Р 34.11-2012 (256 бит)
            "1.2.643.7.1.1.1.1" => CpHashAlgorithmName.GOST3411_2012_256, 

            // ГОСТ Р 34.11-2012 (512 бит)
            "1.2.643.7.1.1.1.2" => CpHashAlgorithmName.GOST3411_2012_512, 

            // ГОСТ Р 34.11-94
            "1.2.643.2.2.19" => CpHashAlgorithmName.GOST3411, 

            // Алгоритмы SHA
            "1.2.840.113549.1.1.1" => CpHashAlgorithmName.SHA1,
            "1.2.840.113549.1.1.5" => CpHashAlgorithmName.SHA256,

            // По умолчанию — ГОСТ 2012 (256 бит)
            _ => CpHashAlgorithmName.GOST3411_2012_256
        };
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
        if (signingKey == null || string.IsNullOrEmpty(password))
        {
            return;
        }

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
    /// <param name="signingKey">Ключ для подписи.</param>
    /// <returns>Подпись в виде массива байтов.</returns>
    private static byte[] CreateSignature(byte[] data, Gost3410_2012_256CryptoServiceProvider signingKey)
        => signingKey.SignData(data);
}