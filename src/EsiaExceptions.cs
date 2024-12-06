using System;

namespace AISGorod.AspNetCore.Authentication.Esia;

/// <summary>
/// Ошибка запроса к REST API ЕСИА.
/// </summary>
internal class EsiaRestRequestException : Exception
{
    public EsiaRestRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
