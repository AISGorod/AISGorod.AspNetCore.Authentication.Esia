using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;

/// <summary>
/// Методы для сертификатов.
/// </summary>
internal static class X509CertificateUtils
{
    /// <summary>
    /// Загрузка сертификата.
    /// </summary>
    /// <param name="pem">Сертификат в формате PEM.</param>
    /// <returns><see cref="X509Certificate2"/>.</returns>
    internal static X509Certificate2 LoadCertificate(string pem) =>
        X509CertificateLoader.LoadCertificate(Encoding.UTF8.GetBytes(pem));
}