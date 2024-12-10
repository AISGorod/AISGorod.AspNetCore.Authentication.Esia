using System;

namespace AISGorod.AspNetCore.Authentication.Esia;

/// <summary>
/// Ошибка запроса к REST API ЕСИА.
/// </summary>
internal class EsiaRestRequestException(string? message, Exception? innerException) : Exception(message, innerException);