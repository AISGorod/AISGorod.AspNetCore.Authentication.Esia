using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Text;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Класс для обеспечения защиты запросов.
    /// Необходим для генерации поля state согласно правилам ЕСИА.
    /// </summary>
    class EsiaSecureDataFormat : ISecureDataFormat<AuthenticationProperties>
    {
        private Dictionary<string, AuthenticationProperties> _dict = new Dictionary<string, AuthenticationProperties>();

        public string Protect(AuthenticationProperties data)
        {
            return Protect(data, null);
        }

        public string Protect(AuthenticationProperties data, string purpose)
        {
            var code = data.Items.ContainsKey(OpenIdConnectDefaults.UserstatePropertiesKey) ? data.Items[OpenIdConnectDefaults.UserstatePropertiesKey] : Guid.NewGuid().ToString();
            _dict.Add(code, data);
            return code;
        }

        public AuthenticationProperties Unprotect(string protectedText)
        {
            return Unprotect(protectedText, null);
        }

        public AuthenticationProperties Unprotect(string protectedText, string purpose)
        {
            if (!_dict.ContainsKey(protectedText))
                return null;
            var data = _dict[protectedText];
            _dict.Remove(protectedText);
            return data;
        }
    }
}
