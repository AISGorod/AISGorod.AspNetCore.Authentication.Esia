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
        public const string DisplayName = "Госуслуги (ЕСИА)";

        /// <summary>
        /// Название ключа в AuthenticationProperties, в котором хранятся текущие scope сессии.
        /// </summary>
        public const string EnablesScopesPropertiesKey = "Esia.EnabledScopes";

        /// <summary>
        /// Имя HTTP-клиента для работы REST API.
        /// </summary>
        public const string RestClientHttpName = "Esia.RestClient";

        /// <summary>
        /// Тип claim для контактных данных ФЛ.
        /// </summary>
        public const string PrnsCttsClaimType = "urn:esia:prns:ctts";

        /// <summary>
        /// Тип claim для адресов ФЛ.
        /// </summary>
        public const string PrnsAddrsClaimType = "urn:esia:prns:addrs";

        /// <summary>
        /// Тип claim для документов ФЛ.
        /// </summary>
        public const string PrnsDocsClaimType = "urn:esia:prns:docs";
    }
}
