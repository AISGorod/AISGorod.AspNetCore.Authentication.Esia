using AISGorod.AspNetCore.Authentication.Esia;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Класс с методами расширения для настройки веб-сайта.
    /// </summary>
    public static class EsiaExtensions
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder)
            => builder.AddEsia(EsiaDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddEsia<TEsiaEvents>(this AuthenticationBuilder builder)
            where TEsiaEvents : OpenIdConnectEvents
            => builder.AddEsia<TEsiaEvents>(EsiaDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, Action<EsiaOptions> configureOptions)
            => builder.AddEsia(EsiaDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddEsia<TEsiaEvents>(this AuthenticationBuilder builder, Action<EsiaOptions> configureOptions)
            where TEsiaEvents : OpenIdConnectEvents
            => builder.AddEsia<TEsiaEvents>(EsiaDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, string authenticationScheme, Action<EsiaOptions> configureOptions)
            => builder.AddEsia(authenticationScheme, EsiaDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddEsia<TEsiaEvents>(this AuthenticationBuilder builder, string authenticationScheme, Action<EsiaOptions> configureOptions)
            where TEsiaEvents : OpenIdConnectEvents
            => builder.AddEsia<TEsiaEvents, EsiaHandler>(authenticationScheme, EsiaDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<EsiaOptions> configureOptions)
            => builder.AddEsia<EsiaEvents, EsiaHandler>(authenticationScheme, displayName, configureOptions);

        public static AuthenticationBuilder AddEsia<TEsiaEvents>(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            string displayName,
            Action<EsiaOptions> configureOptions)
            where TEsiaEvents : OpenIdConnectEvents
            => builder.AddEsia<TEsiaEvents, EsiaHandler>(authenticationScheme, displayName, configureOptions);

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
            IEsiaEnvironment esiaEnvironment = esiaOptions.EnvironmentInstance
                ?? new EsiaEnvironmentResolver(esiaOptions.Environment ?? throw new ArgumentNullException("Environment and EnvironmentInstance is null")).Resolve();

            // register new services
            builder.Services.AddSingleton(esiaOptions);
            builder.Services.AddSingleton(esiaEnvironment);
            builder.Services.AddSingleton<TEsiaEvents>();
            builder.Services.AddSingleton<OpenIdConnectOptionsBuilder>();
            builder.Services.AddTransient<IEsiaRestService, EsiaRestService>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
            builder.Services.AddHttpClient(EsiaDefaults.RestClientHttpName, esiaOptions.RestApiHttpClientHandler ?? ((_) => { }));

            var configBuilder = new OpenIdConnectOptionsBuilder(esiaOptions, esiaEnvironment);
            return builder.AddRemoteScheme<OpenIdConnectOptions, TEsiaHandler>(authenticationScheme, displayName, configBuilder.BuildAction<TEsiaEvents>());
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Класс с методами-расширениями для внутренних дел этой библиотеки.
    /// </summary>
    static class EsiaExtensions
    {
        /// <summary>
        /// Подписывает запрос (или вычисляет client_secret запроса). 
        /// </summary>
        internal static string SignData(IEsiaSigner? esiaSigner, EsiaOptions options, string scope, string timestamp, string clientId, string state)
        {
            byte[] signData = Encoding.UTF8.GetBytes(scope + timestamp + clientId + state);
            return (esiaSigner != null)
                ? esiaSigner.Sign(signData)
                : throw new ArgumentNullException("Need to define IEsiaSigner");
        }
    }
}