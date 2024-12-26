using System.Threading.Tasks;

namespace AISGorod.AspNetCore.Authentication.Esia;

/// <summary>
/// Интерфейс для подписи данных с помощью ключа ИС.
/// </summary>
public interface IEsiaSigner
{
    /// <summary>
    /// Подписать строку при помощи ключа из сертификата ИС.
    /// </summary>
    /// <param name="concatenatedString">Строка, которую необходимо подписать.</param>
    /// <returns>Подпись.</returns>
    Task<string> SignAsync(string concatenatedString);

    /// <summary>
    /// Получение хеш-суммы сертификата.
    /// </summary>
    /// <returns>Хеш-сумма сертификата.</returns>
    Task<string> GetCertificateFingerprintAsync();
}