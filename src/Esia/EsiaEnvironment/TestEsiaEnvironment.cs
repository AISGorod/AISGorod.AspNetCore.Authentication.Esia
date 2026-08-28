using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;

/// <summary>
/// Описание тестовой среды ЕСИА.
/// </summary>
public class TestEsiaEnvironment : IEsiaEnvironment
{
    /// <summary>
    /// Сертификаты тестовой среды ЕСИА: сначала актуальный, затем предыдущие.
    /// Загружаются один раз, так как <see cref="X509Certificate2"/> удерживает неуправляемые ресурсы.
    /// </summary>
    private static readonly X509Certificate2[] Certificates =
    [
        X509CertificateUtils.LoadCertificate(Esia.EsiaCertificates.TestCertificate2024)
    ];

    /// <summary>
    /// Сертификат среды ЕСИА.
    /// </summary>
    [Obsolete("Свойство устарело и будет удалено в следующей мажорной версии. Используйте EsiaCertificates.")]
    public X509Certificate2 EsiaCertificate => Certificates[0];

    /// <summary>
    /// Сертификаты среды ЕСИА.
    /// </summary>
    public IReadOnlyCollection<X509Certificate2> EsiaCertificates => Certificates;

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
}