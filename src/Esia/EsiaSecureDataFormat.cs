using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace AISGorod.AspNetCore.Authentication.Esia;

/// <summary>
/// Класс для обеспечения защиты запросов.
/// Необходим для генерации поля state согласно правилам ЕСИА.
/// </summary>
internal class EsiaSecureDataFormat : ISecureDataFormat<AuthenticationProperties>
{
    /// <summary>
    /// Словарь со свойствами аутентификации.
    /// </summary>
    private readonly Dictionary<string, AuthenticationProperties> dict = new();

    /// <inheritdoc />
    public string Protect(AuthenticationProperties data) => Protect(data, null);

    /// <inheritdoc />
    public string Protect(AuthenticationProperties data, string? purpose)
    {
        var code = data.Items.TryGetValue(OpenIdConnectDefaults.UserstatePropertiesKey, out var item) ? item : Guid.NewGuid().ToString();
        
        if (code == null)
        {
            return string.Empty;
        }
        
        dict.Add(code, data);
        return code;
    }

    /// <inheritdoc />
    public AuthenticationProperties? Unprotect(string? protectedText) => Unprotect(protectedText, null);

    /// <inheritdoc />
    public AuthenticationProperties? Unprotect(string? protectedText, string? purpose)
    {
        if (protectedText != null)
            return !dict.Remove(protectedText, value: out var data) ? null : data;
        return null;
    }
}