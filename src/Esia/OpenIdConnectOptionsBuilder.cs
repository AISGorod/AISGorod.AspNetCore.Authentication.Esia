using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;
using AISGorod.AspNetCore.Authentication.Esia.Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AISGorod.AspNetCore.Authentication.Esia;

/// <summary>
/// Формирует экземпляр класса OpenIdConnectOptions из экземпляра класса EsiaOptions.
/// </summary>
internal class OpenIdConnectOptionsBuilder(EsiaOptions esiaOptions, IEsiaEnvironment environment)
{
    /// <summary>
    /// Применение настроек.
    /// </summary>
    /// <typeparam name="TEsiaEvents">События.</typeparam>
    public Action<OpenIdConnectOptions> BuildAction<TEsiaEvents>() where TEsiaEvents : OpenIdConnectEvents
    {
        return options =>
        {
            ConfigureBackchannel(options);
            ConfigureEndpoints(options);
            ConfigureTokenValidation(options);
            ConfigureClientSettings(options);
            ConfigureEvents<TEsiaEvents>(options);
            ConfigureScopes(options.Scope);
            ConfigureSchemes(options);
        };
    }

    /// <summary>
    /// Настройка backchannel.
    /// </summary>
    /// <param name="options">Настройки openId.</param>
    private void ConfigureBackchannel(OpenIdConnectOptions options)
    {
        if (esiaOptions.Backchannel != null)
        {
            options.Backchannel = esiaOptions.Backchannel;
        }
    }

    /// <summary>
    /// Настройка endpoint-ов.
    /// </summary>
    /// <param name="options">Настройки openId.</param>
    private void ConfigureEndpoints(OpenIdConnectOptions options)
    {
        options.Configuration = new OpenIdConnectConfiguration
        {
            AuthorizationEndpoint = environment.AuthorizationEndpoint,
            TokenEndpoint = environment.TokenEndpoint,
            EndSessionEndpoint = environment.LogoutEndpoint
        };
    }

    /// <summary>
    /// Настройка токена валидации.
    /// </summary>
    /// <param name="options">Настройки openId.</param>
    private void ConfigureTokenValidation(OpenIdConnectOptions options)
    {
        options.TokenValidationParameters.ValidIssuer = environment.Issuer;

        if (esiaOptions.SkipSignatureValidation)
        {
            // Отключаем стандартную проверку подписи ключом потому что у нас это делается на отдельном сервере (тут не сертификата, с открытым ключом), код для проверки будет в OnTokenValidated
            options.TokenValidationParameters.ValidateIssuerSigningKey = false;
            options.TokenValidationParameters.SignatureValidator = (token, parameters) =>
            {
                // Просто возвращаем токен как есть, без локальной проверки подписи
                return new JsonWebToken(token);
            };
        }
        else
        {
            options.TokenValidationParameters.IssuerSigningKey = new RsaSecurityKey(environment.EsiaCertificate.GetRSAPublicKey());
        }
    }

    /// <summary>
    /// Настройка настроек клиента.
    /// </summary>
    /// <param name="options">Настройки openId.</param>
    private void ConfigureClientSettings(OpenIdConnectOptions options)
    {
        options.TokenHandler = esiaOptions.TokenHandler;
        options.ClientId = esiaOptions.Mnemonic;
        options.ProtocolValidator.RequireNonce = false;
        options.GetClaimsFromUserInfoEndpoint = false;
        options.StateDataFormat = new EsiaSecureDataFormat();
        options.SaveTokens = esiaOptions.SaveTokens;
        options.CallbackPath = esiaOptions.CallbackPath;
        options.SignedOutCallbackPath = esiaOptions.SignedOutCallbackPath;
    }

    /// <summary>
    /// Настройка событий.
    /// </summary>
    /// <param name="options">Настройки openId.</param>
    /// <typeparam name="TEsiaEvents">Тип событий ЕСИА.</typeparam>
    private static void ConfigureEvents<TEsiaEvents>(OpenIdConnectOptions options) where TEsiaEvents : OpenIdConnectEvents
    {
        options.EventsType = typeof(TEsiaEvents);
    }

    /// <summary>
    /// Настройка scope-ов.
    /// </summary>
    /// <param name="optionsScope">Scopes.</param>
    private void ConfigureScopes(ICollection<string> optionsScope)
    {
        optionsScope.Clear();
        optionsScope.Add("openid");

        foreach (var scope in esiaOptions.Scope)
        {
            if (!scope.Equals("openid", StringComparison.OrdinalIgnoreCase))
            {
                optionsScope.Add(scope);
            }
        }
    }

    /// <summary>
    /// Настройка схем.
    /// </summary>
    /// <param name="options">Настройки openId.</param>
    private void ConfigureSchemes(OpenIdConnectOptions options)
    {
        options.SignInScheme = esiaOptions.SignInScheme;
        options.SignOutScheme = esiaOptions.SignOutScheme;
    }
}