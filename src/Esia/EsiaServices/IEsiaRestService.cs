using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaServices;

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
    Task<JsonElement> CallAsync(string url, HttpMethod method);

    /// <summary>
    /// Обновляет маркеры доступа и обновления.
    /// Перед вызовами API-методов этот запрос не надо вызывать вручную.
    /// </summary>
    Task RefreshTokensAsync();
}