using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;

/// <summary>
/// Описание тестовой среды ЕСИА.
/// </summary>
public class TestEsiaEnvironment : IEsiaEnvironment
{
    /// <summary>
    /// Сертификаты среды ЕСИА.
    /// </summary>
    public IReadOnlyCollection<X509Certificate2> EsiaCertificates =>
    [
        LoadCertificate(Esia.EsiaCertificates.TestCertificate),
        //LoadCertificate(Esia.EsiaCertificates.Production2025)
    ];

    /// <summary>
    /// Базовый URL для запросов.
    /// </summary>
    public string Host => "https://esia-portal1.test.gosuslugi.ru";

    /// <summary>
    /// Endpoint для получения авторизационного кода.
    /// </summary>
    public string AuthorizationEndpoint => Host + "/aas/oauth2/v2/ac";

    /// <summary>
    /// Endpoint для получения маркера доступа и(или) маркера идентификации.
    /// </summary>
    public string TokenEndpoint => Host + "/aas/oauth2/v3/te";

    /// <summary>
    /// Endpoint для логаута.
    /// </summary>
    public string LogoutEndpoint => Host + "/idp/ext/Logout";

    /// <summary>
    /// Базовый URL для REST-сервиса персональных данных.
    /// </summary>
    public string RestPersonsEndpoint => Host + "/rs/prns/";

    /// <summary>
    /// Issuer маркеров доступа.
    /// </summary>
    public string Issuer => "http://esia-portal1.test.gosuslugi.ru/";
    
    /// <summary>
    /// Загрузка сертификата.
    /// </summary>
    /// <param name="pem">Сертификат.</param>
    /// <returns><see cref="X509Certificate2"/>.</returns>
    private static X509Certificate2 LoadCertificate(string pem) =>
        X509CertificateLoader.LoadCertificate(Encoding.UTF8.GetBytes(pem));
}