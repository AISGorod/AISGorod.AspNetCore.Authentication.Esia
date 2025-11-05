using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;

/// <summary>
/// Методы для сертификатов.
/// </summary>
internal static class X509CertificateUtils
{
    /// <summary>
    /// Загрузка сертификата.
    /// </summary>
    /// <param name="pem">Сертификат.</param>
    /// <returns><see cref="X509Certificate2"/>.</returns>
    internal static X509Certificate2 LoadCertificate(string pem) =>
        X509CertificateLoader.LoadCertificate(Encoding.UTF8.GetBytes(pem));
}


