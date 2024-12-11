using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AISGorod.AspNetCore.Authentication.Esia;

/// <summary>
/// Обработчик запросов (основная часть middleware).
/// </summary>
/// <param name="options">Настройки.</param>
/// <param name="logger">Логгер.</param>
/// <param name="htmlEncoder">Кодировщик html.</param>
/// <param name="encoder">url декодер.</param>
internal abstract class EsiaHandler(
    IOptionsMonitor<OpenIdConnectOptions> options,
    ILoggerFactory logger,
    HtmlEncoder htmlEncoder,
    UrlEncoder encoder)
    : OpenIdConnectHandler(options, logger, htmlEncoder, encoder)
{
    /// <inheritdoc />
    public override async Task<bool> HandleRequestAsync() => await base.HandleRequestAsync();
}