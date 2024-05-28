using System;
using System.Collections.Generic;
using System.Text;

namespace AISGorod.AspNetCore.Authentication.Esia
{
    /// <summary>
    /// Сертификаты тестовой и продуктивной сред ЕСИА, используемые для формирования электронных подписей ответов как поставщика.
    /// Можно взять из архива http://esia.gosuslugi.ru/public/esia.zip.
    /// Ссылка на архив взята из методических рекомендаций ЕСИА, п. 3.1.2 "Аутентификация с использованием OpenID Connect 1.0".
    /// </summary>
    static class EsiaCertificates
    {
        /// <summary>
        /// Сертификат тестовой среды ЕСИА.
        /// </summary>
        public const string TestCertificate = @"-----BEGIN CERTIFICATE-----
MIIDVDCCAjygAwIBAgIEU/3lwTANBgkqhkiG9w0BAQUFADBsMRAwDgYDVQQGEwdV
bmtub3duMRAwDgYDVQQIEwdVbmtub3duMRAwDgYDVQQHEwdVbmtub3duMRAwDgYD
VQQKEwdVbmtub3duMRAwDgYDVQQLEwdVbmtub3duMRAwDgYDVQQDEwdVbmtub3du
MB4XDTE0MDgyNzE0MDU1M1oXDTI0MDgyNDE0MDU1M1owbDEQMA4GA1UEBhMHVW5r
bm93bjEQMA4GA1UECBMHVW5rbm93bjEQMA4GA1UEBxMHVW5rbm93bjEQMA4GA1UE
ChMHVW5rbm93bjEQMA4GA1UECxMHVW5rbm93bjEQMA4GA1UEAxMHVW5rbm93bjCC
ASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBANkcPcRtYPkENEIAfG8EqsSc
TlPOLYxyO7ob8Ap8iUAG/UjS0ZHq2gY5oQNvaAgJ/br+epGmHvWDYaoo0bDVRsiA
AGUwdBB7/fcaiKnaDrb+IqNqOyW8TaJrp+xDazhdmUE+1JKq+G++PBbykhqF9DlX
Ae0q7uh1OzQB9aDSeG2za/Enh3D+AhmbBdtMvy9Do0/kxDRUA/XjaTiLsljy+dhd
WqSiH+bIuMkwcYPUGZQc68LgRkVtWodrJQYyfeRbu0PNrjOWXO6KsBbGIZ4t7x18
Yta19hwrYACngW9ECec9SsDRqLz4QTbZkT1tCUxXl9AoLxzUVLD4JIueunFS2wEC
AwEAATANBgkqhkiG9w0BAQUFAAOCAQEADef396Nt9WUhcbpAFstHGAmuzV2H56DL
O+1O2OgMTn6UB6jDvW3U03uC1dxKWsVM9a3MuoWsy4sNJGLE+Ldnc/7i/XUncJMb
AEwoeL34rFXcRD24mP+0EhleJRcgLHvmLuW/gwe9jVPbFULrc3NpX9kTy8g2e/Nm
OUpQFDHe4mxVXBOFPGyJr4uFDqj4lsZqFlMH0B3uEkaxuRu1Ilt+3fOP7+tpFWVj
KixCHhAuWTOfdlwzEWcRO0QnvqA77vy2fahY44wuarJL3gkMUC5GfHkwb34Cgy08
rNYRhvVqY/YiGqxvrUmNJaOpFxUfEn/UgPHkxIBEtglvY/1jXOB/Yg==
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
