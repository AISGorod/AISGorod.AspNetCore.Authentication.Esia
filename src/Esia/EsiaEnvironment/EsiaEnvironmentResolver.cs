using System;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;

/// <summary>
/// Позволяет получить экземпляр среды ЕИСА на основании значения перечисления.
/// </summary>
/// <param name="type">Тип среды разработки.</param>
internal sealed class EsiaEnvironmentResolver(EsiaEnvironmentType type)
{
    /// <summary>
    /// Определение среды разработки.
    /// </summary>
    /// <returns>Экземпляр класса настроек среды.</returns>
    /// <exception cref="ArgumentException">Неверно указан тип среды выполнения.</exception>
    public IEsiaEnvironment Resolve()
    {
        return type switch
        {
            EsiaEnvironmentType.Test => new TestEsiaEnvironment(),
            EsiaEnvironmentType.Production => new ProductionEsiaEnvironment(),
            _ => throw new ArgumentException(nameof(type))
        };
    }
}