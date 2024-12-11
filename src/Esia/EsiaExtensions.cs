using System;
using System.Text;
using AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;
using AISGorod.AspNetCore.Authentication.Esia.EsiaServices;
using AISGorod.AspNetCore.Authentication.Esia.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AISGorod.AspNetCore.Authentication.Esia;

/// <summary>
/// Класс с методами расширения для настройки веб-сайта.
/// </summary>
public static class EsiaExtensions
{
	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <returns>Builder аутентификации.</returns>
	public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder)
		=> builder.AddEsia(EsiaDefaults.AuthenticationScheme, _ => { });

	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <typeparam name="TEsiaEvents">Класс с событиями ЕСИА.</typeparam>
	/// <returns>Builder аутентификации.</returns>
	public static AuthenticationBuilder AddEsia<TEsiaEvents>(this AuthenticationBuilder builder)
		where TEsiaEvents : OpenIdConnectEvents
		=> builder.AddEsia<TEsiaEvents>(EsiaDefaults.AuthenticationScheme, _ => { });

	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <param name="configureOptions">Настройки ЕСИА.</param>
	/// <returns>Builder аутентификации.</returns>
	public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, Action<IEsiaOptions> configureOptions)
		=> builder.AddEsia(EsiaDefaults.AuthenticationScheme, configureOptions);

	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <param name="configureOptions">Настройки ЕСИА.</param>
	/// <typeparam name="TEsiaEvents">Класс с событиями ЕСИА.</typeparam>
	/// <returns>Builder аутентификации.</returns>
	public static AuthenticationBuilder AddEsia<TEsiaEvents>(this AuthenticationBuilder builder, Action<EsiaOptions> configureOptions)
		where TEsiaEvents : OpenIdConnectEvents
		=> builder.AddEsia<TEsiaEvents>(EsiaDefaults.AuthenticationScheme, configureOptions);

	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <param name="authenticationScheme">Схема аутентификации.</param>
	/// <param name="configureOptions">Настройки ЕСИА.</param>
	/// <returns>Builder аутентификации.</returns>
	public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, string authenticationScheme, Action<EsiaOptions> configureOptions)
		=> builder.AddEsia(authenticationScheme, EsiaDefaults.DisplayName, configureOptions);

	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <param name="authenticationScheme">Схема аутентификации.</param>
	/// <param name="configureOptions">Настройки ЕСИА.</param>
	/// <typeparam name="TEsiaEvents">Класс с событиями ЕСИА.</typeparam>
	/// <returns>Builder аутентификации.</returns>
	public static AuthenticationBuilder AddEsia<TEsiaEvents>(this AuthenticationBuilder builder, string authenticationScheme, Action<EsiaOptions> configureOptions)
		where TEsiaEvents : OpenIdConnectEvents
		=> builder.AddEsia<TEsiaEvents, EsiaHandler>(authenticationScheme, EsiaDefaults.DisplayName, configureOptions);

	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <param name="authenticationScheme">Схема аутентификации.</param>
	/// <param name="displayName">Отображаемое имя.</param>
	/// <param name="configureOptions">Настройки ЕСИА.</param>
	/// <returns>Builder аутентификации.</returns>
	public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<EsiaOptions> configureOptions)
		=> builder.AddEsia<EsiaEvents, EsiaHandler>(authenticationScheme, displayName, configureOptions);

	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <param name="authenticationScheme">Схема аутентификации.</param>
	/// <param name="displayName">Отображаемое имя.</param>
	/// <param name="configureOptions">Настройки ЕСИА.</param>
	/// <typeparam name="TEsiaEvents">Класс с событиями ЕСИА.</typeparam>
	/// <returns>Builder аутентификации.</returns>
	public static AuthenticationBuilder AddEsia<TEsiaEvents>(
		this AuthenticationBuilder builder,
		string authenticationScheme,
		string displayName,
		Action<EsiaOptions> configureOptions)
		where TEsiaEvents : OpenIdConnectEvents
		=> builder.AddEsia<TEsiaEvents, EsiaHandler>(authenticationScheme, displayName, configureOptions);

	/// <summary>
	/// Добавить ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <param name="authenticationScheme">Схема аутентификации.</param>
	/// <param name="displayName">Отображаемое имя.</param>
	/// <param name="configureOptions">Настройки ЕСИА.</param>
	/// <typeparam name="TEsiaEvents">События ЕСИА.</typeparam>
	/// <typeparam name="TEsiaHandler">Обработчик ЕСИА.</typeparam>
	/// <returns>Builder аутентификации.</returns>
	/// <exception cref="ArgumentNullException">Не указан тип среды исполнения.</exception>
	/// <exception cref="InvalidOperationException">Не указан ни один из способов подписи.</exception>
	public static AuthenticationBuilder AddEsia<TEsiaEvents, TEsiaHandler>(
		this AuthenticationBuilder builder,
		string authenticationScheme,
		string displayName,
		Action<EsiaOptions> configureOptions)
		where TEsiaEvents : OpenIdConnectEvents
		where TEsiaHandler : OpenIdConnectHandler
	{
		var esiaOptions = new EsiaOptions();
		configureOptions(esiaOptions);

		var esiaEnvironment = esiaOptions.EnvironmentInstance
		                      ?? new EsiaEnvironmentResolver(esiaOptions.Environment
		                                                     ?? throw new ArgumentNullException(nameof(EsiaOptions.Environment), $"Необходимо указать {nameof(esiaOptions.Environment)} или {nameof(esiaOptions.EnvironmentInstance)}."))
			                      .Resolve();

		// Проверка на наличие настроенной подписи.
		if (esiaOptions.SignerFactory == null)
		{
			throw new InvalidOperationException("Необходимо настроить один из способов подписи.");
		}

		// Регистрация сервисов.
		RegisterEsiaServices(builder, esiaOptions, esiaEnvironment);

		// Создание конфигурации и добавление схемы аутентификации.
		var configBuilder = new OpenIdConnectOptionsBuilder(esiaOptions, esiaEnvironment);
		return builder.AddRemoteScheme<OpenIdConnectOptions, TEsiaHandler>(
			authenticationScheme,
			displayName,
			configBuilder.BuildAction<TEsiaEvents>());
	}

	/// <summary>
	/// Регистрация сервисов для работы с ЕСИА.
	/// </summary>
	/// <param name="builder">Builder аутентификации.</param>
	/// <param name="options">Настройки ЕСИА.</param>
	/// <param name="environment">Настройки среды.</param>
	private static void RegisterEsiaServices(AuthenticationBuilder builder, EsiaOptions options, IEsiaEnvironment environment)
	{
		builder.Services.AddSingleton(provider => options.SignerFactory!(provider));
		builder.Services.AddSingleton(options);
		builder.Services.AddSingleton(environment);
		builder.Services.AddTransient<IEsiaRestService, EsiaRestService>();
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
		builder.Services.AddHttpClient(EsiaDefaults.RestClientHttpName, options.RestApiHttpClientHandler ?? (_ => { }));
	}

	/// <summary>
	/// Подписывает запрос (или вычисляет client_secret запроса). 
	/// </summary>
	/// <param name="esiaSigner">Класс подписи данных.</param>
	/// <param name="scope">Scope.</param>
	/// <param name="timestamp">Время.</param>
	/// <param name="clientId">Идентификатор клиента.</param>
	/// <param name="state">Состояние.</param>
	/// <returns>Подпись.</returns>
	/// <exception cref="ArgumentNullException">Не указан тип подписи.</exception>
	internal static string SignData(IEsiaSigner? esiaSigner, string scope, string timestamp, string clientId, string state)
	{
		if (esiaSigner == null)
		{
			throw new ArgumentNullException(nameof(esiaSigner), $"Необходимо явно задать класс для {nameof(IEsiaSigner)}.");
		}

		var signData = Encoding.UTF8.GetBytes(scope + timestamp + clientId + state);
		return esiaSigner.Sign(signData);
	}
}