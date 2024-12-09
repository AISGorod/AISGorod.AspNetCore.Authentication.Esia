namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Сертификаты тестовой и продуктивной сред ЕСИА, используемые для формирования электронных подписей ответов как поставщика.
    /// Можно взять из архива http://esia.gosuslugi.ru/public/esia.zip.
    /// Ссылка на архив взята из методических рекомендаций ЕСИА, п. 3.1.2 "Аутентификация с использованием OpenID Connect 1.0".
    /// </summary>
    internal static class EsiaCertificates
    {
        /// <summary>
        /// Сертификат тестовой среды ЕСИА.
        /// </summary>
        public const string TestCertificate = @"-----BEGIN CERTIFICATE-----
MIIDGjCCAgKgAwIBAgIEZrHvUjANBgkqhkiG9w0BAQsFADAeMQswCQYDVQQGEwJS
VTEPMA0GA1UECwwGUlRMYWJzMB4XDTI0MDgwNjA5MzkzMFoXDTM0MDgwNjA5Mzkz
MFowHjELMAkGA1UEBhMCUlUxDzANBgNVBAsMBlJUTGFiczCCASIwDQYJKoZIhvcN
AQEBBQADggEPADCCAQoCggEBAL77jUBqvXnVEBiqFzgjvM5AY0VHRfUQkHyuRVws
4fxD6LV8GmxaBOMUN8D/grjhbfcUoQ86G+7q9QOsSaIYGVPa9A0szKlSUGiGXJ8T
mvekIgdv0v4DoNrWe9OTdWKt9hWntZGNqwYXFIjMZsIyCrOFwslWXcbvBlh5dQZ0
IBZC4ybwbLXTssHz73oJEnk2d2Nwf80iRRwOawNbZid3Z0JMuq/8d3NuRsjZ/t8Z
7YOQ4Z3aaiyZlQr2XmzpZX0aXCXVGlbJPtifjXnPU4ItUMnSMGCSHCDp+vD+Z0O4
dcZql8ynHvSur9Zez121cuOOUlLceb/lm55TkgizRTo7TiMCAwEAAaNgMF4wDwYD
VR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBTBVzJnKRNEZOPQTXOQzeE+XMlMKzAd
BgNVHQ4EFgQUwVcyZykTRGTj0E1zkM3hPlzJTCswCwYDVR0PBAQDAgEGMA0GCSqG
SIb3DQEBCwUAA4IBAQB8zT8alwkm/MENoIBpcDbaKMjdYTMsRvVAolwxPUyh9zpv
PCgA7e/WnTqxxXm4tDqJpbIfmpXd1YXYi0ChfyYRBUUYkksofyunXgkqHpZkjs9a
OjkZuQYqQ6F7AcgVSQHfzMEm1I+7D1ruTELZgJaE5KJF1WwI7CEhnFfbJKxgl9Tv
01qtB+FRgrLtyCwJFXBqkl3e9ss6WL90VnbZdkh5/U2og8wQG7GhRdQbRB0ubYKw
612pMGIzfC9P5RSzz3LAZYriUiHBKtVZ2iHGFG+1szUtIG3fT60pgb+/g+FTwQgU
tXZJVX6zc9Y457U/tcp1LMqhyJHfSuLWI8s2oynL
-----END CERTIFICATE-----";

        /// <summary>
        /// Сертификат продукционной среды ЕСИА.
        /// </summary>
        public const string ProductionCertificate = @"-----BEGIN CERTIFICATE-----
MIIC+DCCAeCgAwIBAgIEZkCOZTANBgkqhkiG9w0BAQsFADA+MQswCQYDVQQGEwJS
VTEPMA0GA1UECgwGUlRMYWJzMQ8wDQYDVQQLDAZSVExhYnMxDTALBgNVBAMMBEVT
SUEwHhcNMjQwNTEyMDkzOTQ5WhcNMjUwNTEyMDkzOTQ5WjA+MQswCQYDVQQGEwJS
VTEPMA0GA1UECgwGUlRMYWJzMQ8wDQYDVQQLDAZSVExhYnMxDTALBgNVBAMMBEVT
SUEwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCfZtVPWKNkkDlSyF5C
d8N6YHjj3MBDJ7JAvG/yYGOg9CqIOJ8y0oEZshBEIRlXuTnf6PARNAb0G6KJxjrR
NCw8vxfOhHxDiR/F/bez1Xyp7L9LIibw1kAMCScbAZobuOVekyvYsKC2JWur3KG+
NvS81FVVtLTWkwlIjZozioE7c4aQDAASGWmVCO2YC8TDExqUcgx4Dq+3d5RnfQrk
F+2QmTWPJVk9fvLX8foi+e/aQxR45Y6D6qSVHDkGt6cVt+FzAui1DyiDzRNNiECr
tqAGsIX4Y6uJxx3vhBxMVVSaGBQ2NEj8hmWpPubXhHez+OaXrN7Zy6wXFfgd8XB6
ej3NAgMBAAEwDQYJKoZIhvcNAQELBQADggEBAGJAgggo8qGX8ad+WUjTII/AYP/D
cSxUXSyGPnWXxr3t+7/EWphWbmFBEtI3Q9+mOHc9aIX4ExuaNzTTjZG7M5kXvt0B
54tI6bS9p7dlzAyrVrmGoBMHZWBez7EuexW4L7/gD1S/OV2Wn7XlofraFrAo+Sh3
6My5cwlIOWUzS4Rme7wmQ8oS7FyLdoxhTk81gUHBhxF2JHMToL58Zpx3CTPJRIyX
KDCLEUAg+/ZuPybz8Gq8xViNA+ThFYgbW8HXmCBlCPCrrw/VSHqhDPwnBKuPeO5Z
+mNJEAWLBRo34+8FfCaN+Fn0HNIdOGfwiH2yMFJitnT9ORIFEbDUY7zYV3E=
-----END CERTIFICATE-----
";
    }
}