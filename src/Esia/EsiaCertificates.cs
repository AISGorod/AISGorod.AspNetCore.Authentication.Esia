namespace AISGorod.AspNetCore.Authentication.Esia;

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
MIIC/DCCAeSgAwIBAgIIU+kXiIvlFgcwDQYJKoZIhvcNAQELBQAwPjELMAkGA1UE
BhMCUlUxDzANBgNVBAoTBlJUTGFiczEPMA0GA1UECxMGUlRMYWJzMQ0wCwYDVQQD
EwRFU0lBMB4XDTI1MDQyOTA4MTYwMFoXDTI2MDQyOTA4MTYwMFowPjELMAkGA1UE
BhMCUlUxDzANBgNVBAoTBlJUTGFiczEPMA0GA1UECxMGUlRMYWJzMQ0wCwYDVQQD
EwRFU0lBMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqi0oaWj50Bl1
+87p0sPCFqBAV2jrOy0yLGmroYbMUGeJT93yK7/K60htYR477BYG34Wuz9rvr+0I
urL7mJ0sjYu+BdxH+WzTol08sCB2rFcGbO6hLhfz3l0cyrPCUgvDlBSCx7ZNDby7
OS/6Gm+J4KVKRIJZB57ZYRATpE9iZHj/L58rkxMyN/MFaUkjYaWFMrMfrz+NtRK/
3+sULYhqjkAAx8RUNtJO6het/e1VvwfMNxneVytpHBa0nB4XqiBbHCP7J/jmQWIz
vwwxjtamr0ODUzmh6ufjJFMNuZCn8s7UfIanrI5gKeez7qHkuhHdKHRRhi94TbJP
H9whoF4ZvQIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQAst71aPatAa7Fbp0FbdjNE
sT2MOIiiOwx57PNkId11Ryh1LRkZ98qog9CombS0bXbIAwTYjmmRJ6gDvLIBYssL
fFUA3OWNu+LUqidkNmdLYbwIu4UEJ9+4CsjWLzhHlHVCk1a4VUSPOFWzD2TCkRu+
x6VxibADYg8jzLlk+pWmPW9izZCs7etN0vTfj4eGMP18eNU1jI/oMfiMkUNPdI11
sl0dfK0oSP6vnowT+VyCxnie/rP12UZo4vApDo8Bvk4+KTEIKIYXeUjzivvLuyLS
pUPKADeUSRcek57OpIKm5bR1ezl/0BpjjuLFL4UyN1/tNLEritLyTgQ8E+LJwWiu
-----END CERTIFICATE-----";
}