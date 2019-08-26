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
        /// Продукционная среда ЕСИА.
        /// </summary>
        Production
    }

    /// <summary>
    /// Настройка среды ЕСИА.
    /// </summary>
    interface IEsiaEnvironment
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
            switch (this.type)
            {
                case EsiaEnvironmentType.Test:
                    return new TestEnvironment();
                case EsiaEnvironmentType.Production:
                    return new ProductionEnvironment();
                default:
                    throw new ArgumentException(nameof(type));
            }
        }
    }

    /// <summary>
    /// Описание тестовой среды ЕСИА.
    /// </summary>
    sealed class TestEnvironment : IEsiaEnvironment
    {
        public X509Certificate2 EsiaCertificate => new X509Certificate2(Encoding.UTF8.GetBytes(EsiaCertificates.TestCertificate));

        public string Host => "https://esia-portal1.test.gosuslugi.ru";

        public string AuthorizationEndpoint => Host + "/aas/oauth2/ac";

        public string TokenEndpoint => Host + "/aas/oauth2/te";

        public string LogoutEndpoint => Host + "/idp/ext/Logout";

        public string RestPersonsEndpoint => Host + "/rs/prns/";
    }

    /// <summary>
    /// Описание продуктивной среды ЕСИА.
    /// </summary>
    sealed class ProductionEnvironment : IEsiaEnvironment
    {
        public X509Certificate2 EsiaCertificate => new X509Certificate2(Encoding.UTF8.GetBytes(EsiaCertificates.ProductionCertificate));

        public string Host => "https://esia.gosuslugi.ru";

        public string AuthorizationEndpoint => Host + "/aas/oauth2/ac";

        public string TokenEndpoint => Host + "/aas/oauth2/te";

        public string LogoutEndpoint => Host + "/idp/ext/Logout";

        public string RestPersonsEndpoint => Host + "/rs/prns/";
    }
}
