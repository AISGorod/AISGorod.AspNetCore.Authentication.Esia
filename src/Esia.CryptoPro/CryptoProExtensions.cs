using System;
using AISGorod.AspNetCore.Authentication.Esia;
using AISGorod.AspNetCore.Authentication.Esia.Options;

namespace Esia.CryptoPro;

/// <summary>
/// Расширение для CryptoPro.
/// </summary>
public static class CryptoProExtensions
{
    /// <summary>
    /// Добавление подписи через bouncy castle.
    /// </summary>
    /// <param name="options">Настройки.</param>
    /// <param name="configure">Конфигурация.</param>
    public static void UseCryptoPro(this EsiaOptions options, Action<CryptoProOptions> configure)
    {
        options.UseSigner(_ =>
        {
            var cpOptions = new CryptoProOptions();
            configure.Invoke(cpOptions);
            return new CryptoProEsiaSigner(cpOptions);
        });
    }
}