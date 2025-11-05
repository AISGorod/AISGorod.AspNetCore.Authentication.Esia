using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;

/// <summary>
/// Интерфейс настроек среды ЕСИА.
/// </summary>
public interface IEsiaEnvironment
{
    /// <summary>
    /// Сертификат среды ЕСИА.
    /// Устарело: используйте <see cref="EsiaCertificates"/>.
    /// </summary>
    [Obsolete("Свойство устарело. Используйте EsiaCertificates.")]
    X509Certificate2 EsiaCertificate { get; }

    /// <summary>
    /// Сертификаты среды ЕСИА.
    /// </summary>
    IReadOnlyCollection<X509Certificate2> EsiaCertificates { get; }

    /// <summary>
    /// Базовый URL для запросов.
    /// </summary>
    string Host { get; }

    /// <summary>
    /// Endpoint для получения авторизационного кода.
    /// </summary>
    string AuthorizationEndpoint { get; }

    /// <summary>
    /// Endpoint для получения маркера доступа и(или) маркера идентификации.
    /// </summary>
    string TokenEndpoint { get; }

    /// <summary>
    /// Endpoint для логаута.
    /// </summary>
    string LogoutEndpoint { get; }

    /// <summary>
    /// Базовый URL для REST-сервиса персональных данных.
    /// </summary>
    string RestPersonsEndpoint { get; }

    /// <summary>
    /// Issuer маркеров доступа.
    /// </summary>
    string Issuer { get; }
}