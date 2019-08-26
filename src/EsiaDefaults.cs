using System;
using System.Collections.Generic;
using System.Text;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Константы, используемые по умолчанию.
    /// </summary>
    public static class EsiaDefaults
    {
        /// <summary>
        /// Значение по умолчанию для EsiaOptions.AuthenticationScheme.
        /// </summary>
        public const string AuthenticationScheme = "Esia";

        /// <summary>
        /// Значение по умолчанию для названия поставщика.
        /// </summary>
        public static readonly string DisplayName = "Госуслуги (ЕСИА)";

        /// <summary>
        /// Название ключа в AuthenticationProperties, в котором хранятся текущие scope сессии.
        /// </summary>
        public static readonly string EnablesScopesPropertiesKey = "Esia.EnabledScopes";
    }
}
