using System;
using System.IO;
using AISGorod.AspNetCore.Authentication.Esia;
using AISGorod.AspNetCore.Authentication.Esia.Options;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Rosstandart;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Esia.BouncyCastle;

/// <summary>
/// Настройка подписи через bouncy castle.
/// </summary>
/// <param name="options">Настройки.</param>
public class BouncyCastleEsiaSigner(BouncyCastleOptions options) : IEsiaSigner
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
        if (string.IsNullOrWhiteSpace(options.CertFilePath) || string.IsNullOrWhiteSpace(options.KeyFilePath))
        {
            throw new Exception("Для подключения Bouncy castle необходимо указать путь до ключа и до сертификата.");
        }
        
        var signedData = new CmsSignedDataStreamGenerator();
        var cert = ReadCert(options.CertFilePath);
        var key = ReadPemPrivateKey(options.KeyFilePath);

        var digestOid = cert.SigAlgName switch
        {
            GOST_2012_256 => RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256.Id,
            GOST_2012_512 => RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512.Id,
            _ => throw new Exception("Не поддерживаемый тип хеширования"),
        };

        signedData.AddSigner(key, cert, digestOid);

        signedData.AddCertificate(cert);

        using var memory = new MemoryStream();

        using (var stream = signedData.Open(memory, true))
            stream.Write(data, 0, data.Length);

        return Convert.ToBase64String(memory.ToArray());
    }
    
    /// <summary>
    /// Чтение приватного ключа из файла.
    /// </summary>
    /// <param name="pathToPemFile">Путь до ключа.</param>
    /// <returns>Приватный ключ.</returns>
    private static AsymmetricKeyParameter ReadPemPrivateKey(string pathToPemFile)
    {
        var streamReader = File.OpenText(pathToPemFile);
        var pemReader = new Org.BouncyCastle.Utilities.IO.Pem.PemReader(streamReader);
        var pemObject = pemReader.ReadPemObject();
        var privateKeyInfo = PrivateKeyInfo.GetInstance(pemObject.Content);
        var privateKeyBc = PrivateKeyFactory.CreateKey(privateKeyInfo);

        return privateKeyBc;
    }

    /// <summary>
    /// Получение сертификата.
    /// </summary>
    /// <param name="pathToPemFile">Путь до сертификата.</param>
    /// <returns>Сертификат.</returns>
    private static X509Certificate ReadCert(string pathToPemFile)
    {
        var x509CertificateParser = new X509CertificateParser();
        var cert = x509CertificateParser.ReadCertificate(File.ReadAllBytes(pathToPemFile));

        return cert;
    }
}