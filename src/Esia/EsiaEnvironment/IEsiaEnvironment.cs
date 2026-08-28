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
    /// </summary>
    [Obsolete("Свойство устарело и будет удалено в следующей мажорной версии. Используйте EsiaCertificates.")]
    X509Certificate2 EsiaCertificate { get; }

    /// <summary>
    /// Сертификаты среды ЕСИА, которыми могут быть подписаны маркеры доступа.
    /// Содержит прошлые, текущий и (если он уже известен) будущий сертификаты,
    /// чтобы смена сертификата на стороне ЕСИА не приводила к отказу в обслуживании.
    /// </summary>
    /// <remarks>
    /// Реализация по умолчанию возвращает единственный <see cref="EsiaCertificate"/>
    /// ради обратной совместимости с уже написанными реализациями этого интерфейса.
    /// </remarks>
    IReadOnlyCollection<X509Certificate2> EsiaCertificates
    {
        get
        {
#pragma warning disable CS0618 // Реализация по умолчанию опирается на устаревшее свойство.
            return [EsiaCertificate];
#pragma warning restore CS0618
        }
    }

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