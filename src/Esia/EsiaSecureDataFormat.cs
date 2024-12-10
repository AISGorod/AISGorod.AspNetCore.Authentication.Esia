using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Класс для обеспечения защиты запросов.
    /// Необходим для генерации поля state согласно правилам ЕСИА.
    /// </summary>
    internal class EsiaSecureDataFormat : ISecureDataFormat<AuthenticationProperties>
    {
        private readonly Dictionary<string, AuthenticationProperties> _dict = new();

        public string Protect(AuthenticationProperties data) =>
            Protect(data, null);

        public string Protect(AuthenticationProperties data, string? purpose)
        {
            var code = data.Items.TryGetValue(OpenIdConnectDefaults.UserstatePropertiesKey, out var item) ? item : Guid.NewGuid().ToString();
            if (code != null)
            {
                _dict.Add(code, data);
                return code;
            }

            return string.Empty;
        }

        public AuthenticationProperties? Unprotect(string? protectedText) =>
            Unprotect(protectedText, null);

        public AuthenticationProperties? Unprotect(string? protectedText, string? purpose)
        {
            if (protectedText != null)
                return !_dict.Remove(protectedText, value: out var data) ? null : data;
            return null;
        }
    }
}