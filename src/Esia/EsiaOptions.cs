using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Параметры подключения к ЕСИА.
    /// </summary>
    public class EsiaOptions // : RemoteAuthenticationOptions
    {
        /// <summary>
        /// Подпись настроена.
        /// </summary>
        private bool signerConfigured;
        
        /// <summary>
        /// Фабрика подписей.
        /// </summary>
        internal Func<IServiceProvider, IEsiaSigner>? SignerFactory { get; private set; }
        
        /// <summary>
        /// Среда ЕСИА, с которой происходит подключение.
        /// Необходимо указать или Environment, или EnvironmentInstance.
        /// </summary>
        public EsiaEnvironmentType? Environment { get; set; }

        /// <summary>
        /// Экземпляр класса, в котором заданы настройки среды ЕСИА.
        /// Необходимо указать или Environment, или EnvironmentInstance.
        /// </summary>
        public IEsiaEnvironment? EnvironmentInstance { get; set; }

        /// <summary>
        /// Мнемоника ИС.
        /// </summary>
        public string Mnemonic { get; set; } = string.Empty;

        /// <summary>
        /// Набор scope, которые указаны в заявке на регистрацию системы.
        /// При отсутствии openid он добавляется автоматически.
        /// </summary>
        public ICollection<string> Scope { get; set; } = new List<string>();

        /// <summary>
        /// Экземпляр класса HttpClient, с помощью которого будут выполняться запросы.
        /// Полезно, если требуется настроить работу с прокси.
        /// </summary>
        public HttpClient? Backchannel { get; set; }

        /// <summary>
        /// Переопределение DefaultSignInScheme.
        /// Может быть полезно при использовании нескольких провайдеров (например, вместе с IdentityServer).
        /// </summary>
        public string? SignInScheme { get; set; }

        /// <summary>
        /// Схема, используемая при логауте.
        /// По умолчанию берётся значение из SignInScheme.
        /// </summary>
        public string? SignOutScheme { get; set; }

        /// <summary>
        /// Сохраняет маркеры доступа, идентификации и обновления в параметрах аутентификации.
        /// По умолчанию выключено, чтобы уменьшить размер cookie (зато нельзя выполнять API-запросы).
        /// </summary>
        public bool SaveTokens { get; set; }

        /// <summary>
        /// Запрашивает и сохраняет в claims с типом <see cref="EsiaDefaults.PrnsCttsClaimType"/> сведения о контактных данных ФЛ.
        /// </summary>
        public bool GetPrnsContactInformationOnSignIn { get; set; }

        /// <summary>
        /// Запрашивает и сохраняет в claims с типом <see cref="EsiaDefaults.PrnsAddrsClaimType"/> сведения об адресах ФЛ.
        /// </summary>
        public bool GetPrnsAddressesOnSignIn { get; set; }

        /// <summary>
        /// Запрашивает и сохраняет в claims с типом <see cref="EsiaDefaults.PrnsDocsClaimType"/> сведения о документах ФЛ.
        /// </summary>
        public bool GetPrnsDocumentsOnSignIn { get; set; }

        /// <summary>
        /// Валидатор маркера доступа.
        /// Значение по умолчанию - валидатор JwtSecurityTokenHandler.
        /// </summary>
        public ISecurityTokenValidator SecurityTokenValidator { get; set; } = new JwtSecurityTokenHandler();

        /// <summary>
        /// Если не задан <see cref="Backchannel"/>, то позволяет настроить обработчик HTTP-клиента для сервиса обмена с REST API.
        /// </summary>
        public Action<HttpClient>? RestApiHttpClientHandler { get; set; }

        /// <summary>
        /// Путь обратного вызова для аутентификации.
        /// </summary>
        public PathString CallBackPath { get; set; }
        
        /// <summary>
        /// Путь обратного вызова для выхода.
        /// </summary>
        public PathString SignedOutCallbackPath { get; set; }
        
        /// <summary>
        /// Использовать подпись.
        /// </summary>
        /// <param name="factory">Фабрика.</param>
        public void UseSigner(Func<IServiceProvider, IEsiaSigner> factory)
        {
            if (signerConfigured)
                throw new InvalidOperationException("Можно настроить только один вариант подписи. Множественные варианты подписи не допускаются.");
            
            SignerFactory = factory;
            signerConfigured = true;
        }
    }
}