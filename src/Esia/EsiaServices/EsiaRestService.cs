using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;
using AISGorod.AspNetCore.Authentication.Esia.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaServices;

/// <summary>
/// Реализация сервиса выполнения запросов к сервисам ЕСИА на базе подхода REST.
/// </summary>
/// <param name="httpContextAccessor">Доступ к http контексту.</param>
/// <param name="esiaEnvironment">Настройки среды ЕСИА.</param>
/// <param name="esiaSigner">Класс способа подписи.</param>
/// <param name="optionsMonitor">Настройки openIdConnect.</param>
/// <param name="esiaOptions">Настройки ЕСИА.</param>
/// <param name="httpClientFactory">Фабрика http клиентов.</param>
internal class EsiaRestService(
    IHttpContextAccessor httpContextAccessor,
    IEsiaEnvironment esiaEnvironment,
    IEsiaSigner esiaSigner,
    IOptionsMonitor<OpenIdConnectOptions> optionsMonitor,
    EsiaOptions esiaOptions,
    IHttpClientFactory httpClientFactory)
    : IEsiaRestService
{
    /// <summary>
    /// Контекст http.
    /// </summary>
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));

    ///<inheritdoc />
    public async Task<JsonElement> CallAsync(string url, HttpMethod method)
    {
        try
        {
            await CheckAndRefreshTokensAsync();

            var tokenType = await GetTokenAsync(OpenIdConnectParameterNames.TokenType);
            var accessToken = await GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var client = esiaOptions.Backchannel ?? httpClientFactory.CreateClient(EsiaDefaults.RestClientHttpName);
            var request = new HttpRequestMessage(method, $"{esiaEnvironment.Host}{url}")
            {
                Headers = { Authorization = new AuthenticationHeaderValue(tokenType, accessToken) }
            };

            var responseMessage = await client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();

            var responseStr = await responseMessage.Content.ReadAsStringAsync();
            return JsonDocument.Parse(responseStr).RootElement;
        }
        catch (Exception ex)
        {
            throw new EsiaRestRequestException($"Ошибка при обращении к {url}", ex);
        }
    }

    /// <inheritdoc />
    public async Task RefreshTokensAsync()
    {
        var refreshToken = await GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

        var userResult = await httpContext.AuthenticateAsync();
        var user = userResult.Principal;
        var props = userResult.Properties;

        var scheme = props?.Items[".AuthScheme"];
        if (scheme == null)
        {
            throw new InvalidOperationException("Схема аутентификации не указана.");
        }

        var options = optionsMonitor.Get(scheme);
        var now = DateTimeOffset.Now;

        var scope = string.Join(" ", options.Scope);
        var timestamp = now.ToString("yyyy.MM.dd HH:mm:ss") + " " + now.ToString("zzz").Replace(":", "");
        var state = Guid.NewGuid().ToString();

        var clientId = options.ClientId ?? throw new InvalidOperationException("ClientId не настроен.");

        var clientSecret = esiaSigner.SignData($"{clientId}{scope}{timestamp}{state}{refreshToken}");

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", scope },
            { "timestamp", timestamp },
            { "state", state },
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        });

        var tokenResponse = await options.Backchannel.PostAsync(
            options.Configuration?.TokenEndpoint,
            content,
            httpContext.RequestAborted);

        tokenResponse.EnsureSuccessStatusCode();

        var payload = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync()).RootElement;

        ValidateState(state, payload.GetProperty("state").GetString());

        UpdateTokens(props, payload);

        if (user != null)
        {
            await httpContext.SignInAsync(user, props);
        }
    }

    /// <summary>
    /// Получить токен.
    /// </summary>
    /// <param name="tokenName">Название токена.</param>
    /// <returns>Токен.</returns>
    /// <exception cref="ArgumentNullException">Название отсутствует.</exception>
    private async Task<string> GetTokenAsync(string tokenName)
    {
        var token = await httpContext.GetTokenAsync(tokenName);
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(tokenName, $"{tokenName} отсутствует.");
        }

        return token;
    }

    /// <summary>
    /// Валидация состояния.
    /// </summary>
    /// <param name="expected">Ожидаемое состояние.</param>
    /// <param name="actual">Фактическое состояние.</param>
    /// <exception cref="ArgumentException">Полученное состояние не совпадает с ожидаемым.</exception>
    private static void ValidateState(string expected, string? actual)
    {
        if (expected != actual)
        {
            throw new ArgumentException("Полученное состояние не совпадает с ожидаемым.", nameof(actual));
        }
    }

    /// <summary>
    /// Обновление токенов.
    /// </summary>
    /// <param name="props">Свойства аутентификации.</param>
    /// <param name="payload">Полезная нагрузка.</param>
    private static void UpdateTokens(AuthenticationProperties? props, JsonElement payload)
    {
        if (props == null)
        {
            return;
        }

        props.UpdateTokenValue("access_token", payload.GetProperty("access_token").GetString() ?? string.Empty);
        props.UpdateTokenValue("refresh_token", payload.GetProperty("refresh_token").GetString() ?? string.Empty);

        if (payload.TryGetProperty("expires_in", out var expiresInProperty) &&
            int.TryParse(expiresInProperty.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var expiresIn))
        {
            var expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);
            props.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Проверка и обновление токена.
    /// </summary>
    private async Task CheckAndRefreshTokensAsync()
    {
        var expiresAtToken = await GetTokenAsync("expires_at");
        var expiresAt = DateTimeOffset.ParseExact(expiresAtToken, "o", CultureInfo.InvariantCulture);

        // Проверка с запасом в 1 минуту.
        if (expiresAt.AddMinutes(-1) < DateTimeOffset.Now)
        {
            await RefreshTokensAsync();
        }
    }
}