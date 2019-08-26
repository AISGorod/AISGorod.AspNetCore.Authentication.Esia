using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Обработчик запросов (основная часть middleware).
    /// </summary>
    class EsiaHandler : OpenIdConnectHandler
    {
        private EsiaOptions esiaOptions;
        private IEsiaEnvironment esiaEnvironment;

        public EsiaHandler(
            IOptionsMonitor<OpenIdConnectOptions> options,
            ILoggerFactory logger,
            HtmlEncoder htmlEncoder,
            UrlEncoder encoder,
            ISystemClock clock,
            EsiaOptions esiaOptions,
            IEsiaEnvironment esiaEnvironment)
            : base(options, logger, htmlEncoder, encoder, clock)
        {
            this.esiaOptions = esiaOptions;
            this.esiaEnvironment = esiaEnvironment;
        }

        public override async Task<bool> HandleRequestAsync()
        {
            //if (Context.Request.Path.Value.Contains("/refresh"))
            //{
            //    await RefreshTokenAsync();
            //}
            return await base.HandleRequestAsync();
        }
    }
}
