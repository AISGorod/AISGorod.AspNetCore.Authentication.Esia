using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Параметры подключения к ЕСИА.
    /// </summary>
    public class EsiaOptions// : RemoteAuthenticationOptions
    {
        /// <summary>
        /// Среда ЕСИА, с которой происходит подключение.
        /// Необходимо указать или Environment, или EnvironmentInstance.
        /// </summary>
        public EsiaEnvironmentType? Environment { get; set; }

        /// <summary>
        /// Экземпляр класса, в котором заданы настройки среды ЕСИА.
        /// Необходимо указать или Environment, или EnvironmentInstance.
        /// </summary>
        public IEsiaEnvironment EnvironmentInstance { get; set; }

        /// <summary>
        /// Мнемоника ИС.
        /// </summary>
        public string Mnemonic { get; set; }

        /// <summary>
        /// Набор scope, которые указаны в заявке на регистрацию системы.
        /// При отсутствии openid он добавляется автоматически.
        /// </summary>
        public ICollection<string> Scope { get; set; }

        /// <summary>
        /// Экземпляр класса HttpClient, с помощью которого будут выполняться запросы.
        /// Полезно, если требуется настроить работу с прокси.
        /// </summary>
        public HttpClient Backchannel { get; set; }

        /// <summary>
        /// Переопределение DefaultSignInScheme.
        /// Может быть полезно при использовании нескольких провайдеров (например, вместе с IdentityServer).
        /// </summary>
        public string SignInScheme { get; set; }

        /// <summary>
        /// Схема, используемая при логауте.
        /// По умолчанию берётся значение из SignInScheme.
        /// </summary>
        public string SignOutScheme { get; set; }

        /// <summary>
        /// Сохраняет маркеры доступа, идентификации и обновления в параметрах аутентификации.
        /// По умолчанию выключено, чтобы уменьшить размер cookie (зато нельзя выполнять API-запросы).
        /// </summary>
        public bool SaveTokens { get; set; }
    }
}
