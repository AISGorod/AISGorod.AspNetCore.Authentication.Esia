using AISGorod.AspNetCore.Authentication.Esia;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography.Pkcs;
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

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, Action<EsiaOptions> configureOptions)
            => builder.AddEsia(EsiaDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, string authenticationScheme, Action<EsiaOptions> configureOptions)
            => builder.AddEsia(authenticationScheme, EsiaDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<EsiaOptions> configureOptions)
        {
            var esiaOptions = new EsiaOptions();
            configureOptions(esiaOptions);
            IEsiaEnvironment esiaEnvironment = esiaOptions.EnvironmentInstance
                ?? new EsiaEnvironmentResolver(esiaOptions.Environment ?? throw new ArgumentNullException("Environment and EnvironmentInstance is null")).Resolve();

            // register new services
            builder.Services.AddSingleton(esiaOptions);
            builder.Services.AddSingleton(esiaEnvironment);
            builder.Services.AddSingleton<EsiaEvents>();
            builder.Services.AddSingleton<OpenIdConnectOptionsBuilder>();
            builder.Services.AddTransient<IEsiaRestService, EsiaRestService>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());

            var configBuilder = new OpenIdConnectOptionsBuilder(esiaOptions, esiaEnvironment);
            return builder.AddRemoteScheme<OpenIdConnectOptions, EsiaHandler>(authenticationScheme, displayName, configBuilder.BuildAction());
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
        public static string SignData(this EsiaOptions options, string scope, string timestamp, string clientId, string state)
        {
            byte[] signData = Encoding.UTF8.GetBytes(scope + timestamp + clientId + state);
            using (var certPfx = options.Certificate())
            {
                var contentInfo = new ContentInfo(signData);
                var signedCms = new SignedCms(contentInfo, true);
                var cmsSigner = new CmsSigner(certPfx);
                signedCms.ComputeSignature(cmsSigner);
                byte[] signedData = signedCms.Encode();
                return Convert.ToBase64String(signedData);
            }
        }
    }
}