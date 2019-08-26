using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Формирует экземпляр класса OpenIdConnectOptions из экземпляра класса EsiaOptions.
    /// </summary>
    class OpenIdConnectOptionsBuilder
    {
        private EsiaOptions esiaOptions;
        private IEsiaEnvironment environment;

        public OpenIdConnectOptionsBuilder(EsiaOptions esiaOptions, IEsiaEnvironment environment)
        {
            this.esiaOptions = esiaOptions;
            this.environment = environment;
        }

        public Action<OpenIdConnectOptions> BuildAction()
        {
            return (OpenIdConnectOptions options) =>
            {
                options.Backchannel = esiaOptions.Backchannel;
                options.Configuration = new OpenIdConnectConfiguration()
                {
                    AuthorizationEndpoint = environment.AuthorizationEndpoint,
                    TokenEndpoint = environment.TokenEndpoint,
                    EndSessionEndpoint = environment.LogoutEndpoint
                };
                options.TokenValidationParameters.IssuerSigningKey = new RsaSecurityKey(environment.EsiaCertificate.GetRSAPublicKey());
                options.TokenValidationParameters.ValidIssuer = "http://esia.gosuslugi.ru/";
                options.ClientId = esiaOptions.Mnemonic;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = false;
                options.StateDataFormat = new EsiaSecureDataFormat();
                options.EventsType = typeof(EsiaEvents);
                _FillScopes(options.Scope);
            };
        }

        private void _FillScopes(ICollection<string> optionsScope)
        {
            optionsScope.Clear();
            optionsScope.Add("openid");
            if (esiaOptions.Scope != null)
            {
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
}
