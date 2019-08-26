using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Сервис выполнения запросов к сервисам ЕСИА на базе подхода REST.
    /// </summary>
    public interface IEsiaRestService
    {
        /// <summary>
        /// Отправляет запрос.
        /// Если требуется перевыпуск маркера доступа, данный запрос делается автоматически.
        /// </summary>
        /// <param name="url">Путь до веб-сервиса относительно корня сайта. Например, /rs/prns/1000?ctts</param>
        /// <param name="method">Метод, которым</param>
        /// <returns></returns>
        Task<JObject> CallAsync(string url, HttpMethod method);

        /// <summary>
        /// Обновляет маркеры доступа и обновления.
        /// Перед вызовами API-методов этот запрос не надо вызывать вручную.
        /// </summary>
        /// <returns></returns>
        Task RefreshTokensAsync();
    }

    /// <summary>
    /// Реализация сервиса выполнения запросов к сервисам ЕСИА на базе подхода REST.
    /// </summary>
    class EsiaRestService : IEsiaRestService
    {
        private HttpContext context;
        private IEsiaEnvironment esiaEnvironment;
        private IOptionsMonitor<OpenIdConnectOptions> optionsMonitor;
        private EsiaOptions esiaOptions;

        public EsiaRestService(
            IHttpContextAccessor httpContextAccessor,
            IEsiaEnvironment esiaEnvironment,
            IOptionsMonitor<OpenIdConnectOptions> optionsMonitor,
            EsiaOptions esiaOptions)
        {
            this.context = httpContextAccessor.HttpContext;
            this.esiaEnvironment = esiaEnvironment;
            this.optionsMonitor = optionsMonitor;
            this.esiaOptions = esiaOptions;
        }

        public async Task<JObject> CallAsync(string url, HttpMethod method)
        {
            await CheckAndRefreshTokensAsync();

            var tokenType = await context.GetTokenAsync(OpenIdConnectParameterNames.TokenType);
            var accessToken = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if (string.IsNullOrEmpty(tokenType))
            {
                throw new ArgumentNullException(nameof(tokenType));
            }
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            var client = esiaOptions.Backchannel ?? new HttpClient();
            var request = new HttpRequestMessage(method, esiaEnvironment.Host + url);
            request.Headers.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var responseMessage = await client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
            var responseStr = await responseMessage.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseStr);

            return responseJson;
        }

        public async Task RefreshTokensAsync()
        {
            var refreshToken = await context.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            // This is what [Authorize] calls
            var userResult = await context.AuthenticateAsync();
            var user = userResult.Principal;
            var props = userResult.Properties;

            var options = optionsMonitor.Get(props.Items[".AuthScheme"]);

            var now = DateTimeOffset.Now;

            var scope = string.Join(" ", options.Scope);
            var timestamp = now.ToString("yyyy.MM.dd HH:mm:ss") + " " + now.ToString("zzz").Replace(":", "");
            var clientId = options.ClientId;
            var state = Guid.NewGuid().ToString();

            var clientSecret = esiaOptions.SignData(scope, timestamp, clientId, state);

            var pairs = new Dictionary<string, string>()
            {
                { "client_id", options.ClientId },
                { "client_secret", clientSecret },
                { "scope", scope },
                { "timestamp", timestamp },
                { "state", state },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };
            var content = new FormUrlEncodedContent(pairs);
            var tokenResponse = await options.Backchannel.PostAsync(options.Configuration.TokenEndpoint, content, context.RequestAborted);
            tokenResponse.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await tokenResponse.Content.ReadAsStringAsync());

            var stateReceived = payload.Value<string>("state");
            if (state != stateReceived)
            {
                throw new ArgumentException(nameof(state));
            }

            props.UpdateTokenValue("access_token", payload.Value<string>("access_token"));
            props.UpdateTokenValue("refresh_token", payload.Value<string>("refresh_token"));
            if (int.TryParse(payload.Value<string>("expires_in"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var seconds))
            {
                var expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(seconds);
                props.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));
            }
            await context.SignInAsync(user, props);
        }

        private async Task CheckAndRefreshTokensAsync()
        {
            var expiresAtToken = await context.GetTokenAsync("expires_at");
            if (string.IsNullOrEmpty(expiresAtToken))
            {
                throw new ArgumentNullException(nameof(expiresAtToken));
            }
            var expiresAt = DateTimeOffset.ParseExact(expiresAtToken, "o", CultureInfo.InvariantCulture);
            if (expiresAt.AddMinutes(-1) < DateTimeOffset.Now)
            {
                // 1 минуту от времени протухания отнимаем на всякий случай.
                await RefreshTokensAsync();
            }
        }
    }
}
