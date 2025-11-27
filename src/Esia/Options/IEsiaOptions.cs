using System;
using System.Collections.Generic;
using System.Net.Http;
using AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace AISGorod.AspNetCore.Authentication.Esia.Options;

/// <summary>
/// Интерфейс настроек ЕСИА.
/// </summary>
public interface IEsiaOptions
{
    /// <summary>
    /// Среда ЕСИА, с которой происходит подключение.
    /// Необходимо указать или Environment, или EnvironmentInstance.
    /// </summary>
    public EsiaEnvironmentType? Environment { get; }

    /// <summary>
    /// Экземпляр класса, в котором заданы настройки среды ЕСИА.
    /// Необходимо указать или Environment, или EnvironmentInstance.
    /// </summary>
    public IEsiaEnvironment? EnvironmentInstance { get; }

    /// <summary>
    /// Мнемоника ИС.
    /// </summary>
    public string Mnemonic { get; }

    /// <summary>
    /// Набор scope, которые указаны в заявке на регистрацию системы.
    /// При отсутствии openid он добавляется автоматически.
    /// </summary>
    public ICollection<string> Scope { get; }

    /// <summary>
    /// Экземпляр класса HttpClient, с помощью которого будут выполняться запросы.
    /// Полезно, если требуется настроить работу с прокси.
    /// </summary>
    public HttpClient? Backchannel { get; }

    /// <summary>
    /// Переопределение DefaultSignInScheme.
    /// Может быть полезно при использовании нескольких провайдеров (например, вместе с IdentityServer).
    /// </summary>
    public string? SignInScheme { get; }

    /// <summary>
    /// Схема, используемая при логауте.
    /// По умолчанию берётся значение из SignInScheme.
    /// </summary>
    public string? SignOutScheme { get; }

    /// <summary>
    /// Сохраняет маркеры доступа, идентификации и обновления в параметрах аутентификации.
    /// По умолчанию выключено, чтобы уменьшить размер cookie (зато нельзя выполнять API-запросы).
    /// </summary>
    public bool SaveTokens { get; }

    /// <summary>
    /// Запрашивает и сохраняет в claims с типом <see cref="EsiaDefaults.PrnsCttsClaimType"/> сведения о контактных данных ФЛ.
    /// </summary>
    public bool GetPrnsContactInformationOnSignIn { get; }

    /// <summary>
    /// Запрашивает и сохраняет в claims с типом <see cref="EsiaDefaults.PrnsAddrsClaimType"/> сведения об адресах ФЛ.
    /// </summary>
    public bool GetPrnsAddressesOnSignIn { get; }

    /// <summary>
    /// Запрашивает и сохраняет в claims с типом <see cref="EsiaDefaults.PrnsDocsClaimType"/> сведения о документах ФЛ.
    /// </summary>
    public bool GetPrnsDocumentsOnSignIn { get; }

    /// <summary>
    /// Определяет свойства, общие для всех обработчиков токенов безопасности.
    /// Значение по умолчанию - валидатор JwtSecurityTokenHandler.
    /// </summary>
    public TokenHandler TokenHandler { get; }

    /// <summary>
    /// Если не задан <see cref="Backchannel"/>, то позволяет настроить обработчик HTTP-клиента для сервиса обмена с REST API.
    /// </summary>
    public Action<HttpClient>? RestApiHttpClientHandler { get; }

    /// <summary>
    /// Путь обратного вызова для аутентификации.
    /// </summary>
    public PathString CallbackPath { get; }

    /// <summary>
    /// Путь обратного вызова для выхода.
    /// </summary>
    public PathString SignedOutCallbackPath { get; }

    /// <summary>
    /// Если для этого параметра установлено значение <c>true</c>, проверка подписи токена будет пропущена. 
    /// В этом случае вы должны самостоятельно реализовать проверку подписи внутри обработчика события  <see cref="OpenIdConnectEvents.TokenValidated"/>.
    /// </summary>
    public bool SkipSignatureValidation { get; }

    /// <summary>
    /// Использовать обработчик подписи.
    /// </summary>
    /// <param name="factory">Фабрика.</param>
    public void UseSigner(Func<IServiceProvider, IEsiaSigner> factory);

    /// <summary>
    /// Использовать свой обработчик подписи.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="lifetime">Время жизни.</param>
    /// <typeparam name="TSigner">Тип класса для подписи.</typeparam>
    public void UseSigner<TSigner>(IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TSigner : class, IEsiaSigner;
}