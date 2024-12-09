using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using CryptoPro.Security.Cryptography;
using CryptoPro.Security.Cryptography.Pkcs;
using CryptoPro.Security.Cryptography.X509Certificates;

namespace AISGorod.AspNetCore.Authentication.Esia.CryptoPro;

/// <summary>
/// Простейшая обёртка подписи запросов над КриптоПро.
/// </summary>
public class CryptoProEsiaSigner(CryptoProOptions options) : IEsiaSigner
{
    /// <inheritdoc />
    public string Sign(byte[] data)
    {
        if (string.IsNullOrWhiteSpace(options.CertThumbprint) || string.IsNullOrWhiteSpace(options.CertPin))
        {
            throw new Exception($"Для подключения crypto pro необходимо указать {nameof(options.CertThumbprint)} и {nameof(options.CertPin)}");
        }

        using var gostCert = GetGostX509Certificate2(options.CertThumbprint);
        var signingKey = gostCert.GetGost3410_2012_256PrivateKey();
        var signingKeyProvider = signingKey as Gost3410_2012_256CryptoServiceProvider;
        if (signingKeyProvider != null)
        {
            var passwordString = new SecureString();
            foreach (var s in options.CertPin)
                passwordString.AppendChar(s);
            signingKeyProvider.SetContainerPassword(passwordString);
        }

        var contentInfo = new ContentInfo(data);
        var signedCms = new CpSignedCms(contentInfo, true);
        var cmsSigner = new CpCmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, gostCert, signingKeyProvider);

        cmsSigner.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.Now));
        cmsSigner.SignedAttributes.Add(new PkcsSigningCertificateV2(gostCert));

        signedCms.ComputeSignature(cmsSigner);
        var signature = signedCms.Encode();

        return Convert.ToBase64String(signature);
    }

    private static CpX509Certificate2 GetGostX509Certificate2(string certThumbprint)
    {
        using (var store = new CpX509Store(StoreName.My, StoreLocation.LocalMachine))
        {
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);
            if (certs.Count == 1)
                return certs[0];
        }

        using (var store = new CpX509Store(StoreName.My, StoreLocation.CurrentUser))
        {
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);
            if (certs.Count == 1)
                return certs[0];
        }
        
        throw new CryptographicException("Сертификат не найден");
    }
}