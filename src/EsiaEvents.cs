using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Класс, реализующий обработчики событий от поставщика данных.
    /// </summary>
    public class EsiaEvents : OpenIdConnectEvents
    {
        /// <summary>
        /// Свойства подключения к ЕСИА.
        /// </summary>
        protected readonly EsiaOptions EsiaOptions;

        /// <summary>
        /// Среда ЕСИА.
        /// </summary>
        protected readonly IEsiaEnvironment EsiaEnvironment;

        /// <summary>
        /// Сервис подписи данных для взаимодействия с ЕСИА.
        /// </summary>
        protected readonly IEsiaSigner EsiaSigner;

        /// <param name="esiaOptions">Свойства подключения к ЕСИА.</param>
        /// <param name="esiaEnvironment">Среда ЕСИА.</param>
        /// <param name="serviceProvider">Поставщик служб (сейчас используется для получения IEsiaSigner).</param>
        public EsiaEvents(
            EsiaOptions esiaOptions,
            IEsiaEnvironment esiaEnvironment,
            IServiceProvider serviceProvider) // TODO add IEsiaSigner directly
        {
            this.EsiaOptions = esiaOptions;
            this.EsiaEnvironment = esiaEnvironment;
            this.EsiaSigner = serviceProvider.GetService<IEsiaSigner>(); // TODO add IEsiaSigner directly
        }

        /// <summary>
        /// Добавление дополнительных параметров в запрос получения авторизационного кода
        /// </summary>
        protected virtual Task AddAdditionalParametersForReceivingAccessCode(IDictionary<string, string> parameters) =>
            Task.CompletedTask;

        /// <summary>
        /// Событие перенаправления к поставщику данных.
        /// </summary>
        public override Task RedirectToIdentityProvider(RedirectContext context)
        {
            // prepare data
            var now = DateTimeOffset.Now;
            var pm = context.ProtocolMessage;

            // add additional fields for redirect
            pm.ResponseType = OpenIdConnectResponseType.Code;
            pm.Parameters.Add("access_type", "offline");
            pm.Parameters.Add("timestamp", now.ToString("yyyy.MM.dd HH:mm:ss") + " " + now.ToString("zzz").Replace(":", ""));
            pm.State = Guid.NewGuid().ToString();

            // get data for sign
            var scope = pm.Parameters["scope"];
            var timestamp = pm.Parameters["timestamp"];
            var clientId = pm.ClientId;
            var state = pm.State;

            // set clientSecret
            pm.ClientSecret = EsiaExtensions.SignData(EsiaSigner, EsiaOptions, scope, timestamp, clientId, state);

            return AddAdditionalParametersForReceivingAccessCode(pm.Parameters);
        }

        /// <summary>
        /// Событие получения маркера доступа и(или) маркера идентификации.
        /// </summary>
        public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            // prepare data
            var now = DateTimeOffset.Now;
            var pm = context.TokenEndpointRequest;

            // add additional fields for redirect
            pm.ClientId = context.Options.ClientId;
            pm.Parameters.Add("scope", string.Join(" ", (context.Properties as OpenIdConnectChallengeProperties)?.Scope ?? context.Options.Scope));
            pm.Parameters.Add("timestamp", now.ToString("yyyy.MM.dd HH:mm:ss") + " " + now.ToString("zzz").Replace(":", ""));
            pm.State = Guid.NewGuid().ToString();

            // get data for sign
            var scope = pm.Parameters["scope"];
            var timestamp = pm.Parameters["timestamp"];
            var clientId = pm.ClientId;
            var state = pm.State;

            // set clientSecret
            pm.ClientSecret = EsiaExtensions.SignData(EsiaSigner, EsiaOptions, scope, timestamp, clientId, state);

            // ok
            return Task.CompletedTask;
        }

        /// <summary>
        /// Событие получения маркера доступа.
        /// </summary>
        public override async Task TokenValidated(TokenValidatedContext context)
        {
            // We cannot use default UserInfoEndpoint because there are {oId} in uri, BLYATTTT!

            var userOid = context.SecurityToken.Subject;
            var httpClient = context.Options.Backchannel;

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, EsiaEnvironment.RestPersonsEndpoint + userOid);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue(context.TokenEndpointResponse.TokenType, context.TokenEndpointResponse.AccessToken);
            var prnsResult = await httpClient.SendAsync(httpRequest);
            if (prnsResult.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(await prnsResult.Content.ReadAsStringAsync());
                var claimsAction = new MapAllClaimsAction();
                claimsAction.Run(doc.RootElement, context.Principal.Identity as ClaimsIdentity, "esia_prns");
            }

            context.Properties.SetString(EsiaDefaults.EnablesScopesPropertiesKey, string.Join(" ", (context.Properties as OpenIdConnectChallengeProperties)?.Scope ?? context.Options.Scope));
        }

        /// <summary>
        /// Событие перенаправления к поставщику данных для логаута.
        /// </summary>
        public override Task RedirectToIdentityProviderForSignOut(RedirectContext context)
        {
            var pm = context.ProtocolMessage;
            pm.ClientId = context.Options.ClientId;
            pm.PostLogoutRedirectUri = null;
            pm.Parameters.Add("redirect_url", context.Properties.RedirectUri); // THERE ARE DEFAULT redirect_uri param, blyat!!!
            return Task.CompletedTask;
        }
    }
}
