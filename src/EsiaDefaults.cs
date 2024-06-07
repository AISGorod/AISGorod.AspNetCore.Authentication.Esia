namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    ///     Константы, используемые по умолчанию.
    /// </summary>
    public static class EsiaDefaults
    {
        /// <summary>
        ///     Значение по умолчанию для EsiaOptions.AuthenticationScheme.
        /// </summary>
        public const string AUTHENTICATION_SCHEME = "Esia";

        /// <summary>
        ///     Значение по умолчанию для названия поставщика.
        /// </summary>
        public const string DISPLAY_NAME = "Госуслуги (ЕСИА)";

        /// <summary>
        ///     Название ключа в AuthenticationProperties, в котором хранятся текущие scope сессии.
        /// </summary>
        public const string ENABLES_SCOPES_PROPERTIES_KEY = "Esia.EnabledScopes";
    }
}