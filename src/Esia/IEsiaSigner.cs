namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Интерфейс для подписи данных с помощью ключа ИС.
    /// </summary>
    public interface IEsiaSigner
    {
        /// <summary>
        /// Подписать последовательность байт при помощи ключа из сертификата ИС.
        /// </summary>
        /// <param name="data">Данные для подписи.</param>
        /// <returns>Откреплённая подпись.</returns>
        string Sign(byte[] data);
    }
}