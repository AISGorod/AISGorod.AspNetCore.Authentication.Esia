using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Тип среды ЕСИА.
    /// </summary>
    public enum EsiaEnvironmentType
    {
        /// <summary>
        /// Тестовая среда ЕСИА.
        /// </summary>
        Test,

        /// <summary>
        /// Продуктивная среда ЕСИА.
        /// </summary>
        Production,
    }

    /// <summary>
    /// Настройка среды ЕСИА.
    /// </summary>
    public interface IEsiaEnvironment
    {
        /// <summary>
        /// Сертификат среды ЕСИА.
        /// </summary>
        X509Certificate2 EsiaCertificate { get; }

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

    /// <summary>
    /// Описание тестовой среды ЕСИА.
    /// </summary>
    public class TestEsiaEnvironment : IEsiaEnvironment
    {
        /// <summary>
        /// Сертификат среды ЕСИА.
        /// </summary>
        public X509Certificate2 EsiaCertificate => new(Encoding.UTF8.GetBytes(EsiaCertificates.TestCertificate));

        /// <summary>
        /// Базовый URL для запросов.
        /// </summary>
        public string Host => "https://esia-portal1.test.gosuslugi.ru";

        /// <summary>
        /// Endpoint для получения авторизационного кода.
        /// </summary>
        public string AuthorizationEndpoint => Host + "/aas/oauth2/ac";

        /// <summary>
        /// Endpoint для получения маркера доступа и(или) маркера идентификации.
        /// </summary>
        public string TokenEndpoint => Host + "/aas/oauth2/te";

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

    /// <summary>
    /// Описание продуктивной среды ЕСИА.
    /// </summary>
    public class ProductionEsiaEnvironment : IEsiaEnvironment
    {
        /// <summary>
        /// Сертификат среды ЕСИА.
        /// </summary>
        public X509Certificate2 EsiaCertificate => new(Encoding.UTF8.GetBytes(EsiaCertificates.ProductionCertificate));

        /// <summary>
        /// Базовый URL для запросов.
        /// </summary>
        public string Host => "https://esia.gosuslugi.ru";

        /// <summary>
        /// Endpoint для получения авторизационного кода.
        /// </summary>
        public string AuthorizationEndpoint => Host + "/aas/oauth2/ac";

        /// <summary>
        /// Endpoint для получения маркера доступа и(или) маркера идентификации.
        /// </summary>
        public string TokenEndpoint => Host + "/aas/oauth2/te";

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
        public string Issuer => "http://esia.gosuslugi.ru/";
    }

    /// <summary>
    /// Позволяет получить экземпляр среды ЕИСА на основании значения перечисления.
    /// </summary>
    sealed class EsiaEnvironmentResolver
    {
        private EsiaEnvironmentType type;

        public EsiaEnvironmentResolver(EsiaEnvironmentType type)
        {
            this.type = type;
        }

        public IEsiaEnvironment Resolve()
        {
            return type switch
            {
                EsiaEnvironmentType.Test => new TestEsiaEnvironment(),
                EsiaEnvironmentType.Production => new ProductionEsiaEnvironment(),
                _ => throw new ArgumentException(nameof(type))
            };
        }
    }
}
