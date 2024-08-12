using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Формирует экземпляр класса OpenIdConnectOptions из экземпляра класса EsiaOptions.
    /// </summary>
    internal class OpenIdConnectOptionsBuilder
    {
        private readonly IEsiaEnvironment environment;
        private readonly EsiaOptions esiaOptions;

        public OpenIdConnectOptionsBuilder(EsiaOptions esiaOptions, IEsiaEnvironment environment)
        {
            this.esiaOptions = esiaOptions;
            this.environment = environment;
        }

        public Action<OpenIdConnectOptions> BuildAction()
        {
            return BuildAction<EsiaEvents>();
        }

        public Action<OpenIdConnectOptions> BuildAction<TEsiaEvents>() where TEsiaEvents : OpenIdConnectEvents
        {
            return options =>
            {
                options.Backchannel = esiaOptions.Backchannel;
                options.Configuration = new OpenIdConnectConfiguration
                {
                    AuthorizationEndpoint = environment.AuthorizationEndpoint,
                    TokenEndpoint = environment.TokenEndpoint,
                    EndSessionEndpoint = environment.LogoutEndpoint
                };
                options.TokenValidationParameters.IssuerSigningKey = new RsaSecurityKey(environment.EsiaCertificate.GetRSAPublicKey());
                options.TokenValidationParameters.ValidIssuer = "http://esia.gosuslugi.ru/";
                options.SecurityTokenValidator = esiaOptions.SecurityTokenValidator;
                options.ClientId = esiaOptions.Mnemonic;
                options.ProtocolValidator.RequireNonce = false;
                options.GetClaimsFromUserInfoEndpoint = false;
                options.StateDataFormat = new EsiaSecureDataFormat();
                options.EventsType = typeof(TEsiaEvents);
                _FillScopes(options.Scope);
                options.SignInScheme = esiaOptions.SignInScheme;
                options.SignOutScheme = esiaOptions.SignOutScheme;
                options.SaveTokens = esiaOptions.SaveTokens;
            };
        }

        private void _FillScopes(ICollection<string> optionsScope)
        {
            optionsScope.Clear();
            optionsScope.Add("openid");
            
            if (esiaOptions.Scope == null)
            {
                return;
            }
            
            foreach (var scope in esiaOptions.Scope)
            {
                if (!scope.Equals("openid", StringComparison.OrdinalIgnoreCase))
                {
                    optionsScope.Add(scope);
                }
            }
        }
    }
}