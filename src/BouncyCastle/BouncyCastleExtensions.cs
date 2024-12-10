using System;

namespace AISGorod.AspNetCore.Authentication.Esia.BouncyCastle;

/// <summary>
/// Расширение для BouncyCastle.
/// </summary>
public static class BouncyCastleExtensions
{
    /// <summary>
    /// Использовать bouncy castle для подписи.
    /// </summary>
    /// <remarks>
    /// Необходимо использовать только один обработчик для подписи.
    /// </remarks>
    /// <param name="options">Настройки.</param>
    /// <param name="configure">Конфигурация.</param>
    public static void UseBouncyCastle(this EsiaOptions options, Action<BouncyCastleOptions> configure)
    {
        options.UseSigner(_ =>
        {
            var bcOptions = new BouncyCastleOptions();
            configure.Invoke(bcOptions);
            return new BouncyCastleEsiaSigner(bcOptions);
        });
    }
}