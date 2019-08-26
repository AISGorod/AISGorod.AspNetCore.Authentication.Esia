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
MIIDVDCCAjygAwIBAgIEU4b7XDANBgkqhkiG9w0BAQUFADBsMRAwDgYDVQQGEwdV
bmtub3duMRAwDgYDVQQIEwdVbmtub3duMRAwDgYDVQQHEwdVbmtub3duMRAwDgYD
VQQKEwdVbmtub3duMRAwDgYDVQQLEwdVbmtub3duMRAwDgYDVQQDEwdVbmtub3du
MB4XDTE0MDUyOTA5MTgyMFoXDTI0MDUyNjA5MTgyMFowbDEQMA4GA1UEBhMHVW5r
bm93bjEQMA4GA1UECBMHVW5rbm93bjEQMA4GA1UEBxMHVW5rbm93bjEQMA4GA1UE
ChMHVW5rbm93bjEQMA4GA1UECxMHVW5rbm93bjEQMA4GA1UEAxMHVW5rbm93bjCC
ASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAIOP1eIjY2KwuIAUjOzpQ22B
zaqcn1GJKEaGfhhnE1P1XeO7K0y5YRQA7U3AuBmp4E2Padc6ZtQxju54VUW+iFCl
rePY8EQKbx9pP76ODZaLum8KmXnoNQVSWURgR+VLZ2eZOYEd6isp7W/kaRt7AFn2
UInB+sn6FmwUusqXydBCexjcWngJ+WpI0mDMBceJkVRtqWKPi8to43eV5W1oapzJ
XurETr0eMu0mrnaltgYO7BP/Ga2BiUQXYDJ+XfNzOrsThIaeMqfEz9/jZ1wMSJHi
uCDGgTMM38Pzho20vGv1DJzdRDE+G5F25NM/2P+YLiNh9TK64LCELOlUKK3/Sc8C
AwEAATANBgkqhkiG9w0BAQUFAAOCAQEAI3yE1yF+ldMYM9ZBIeSB0LC2BsfTS7pX
8Vl0gFATljnsOzcXPITdjf3pJmyi7B+AMKW6A4JqrMKRBr92FHw7CJccqZ6O5MWj
O0ca7oDHXNin+WeyrzNZajkoLXR7Ah1RzGtsFnF/tKGL9ecPfIZG7G6rpt3SknrA
cB1rmK+0auDphnvvECkCLx/MzPCbTHdqJC9no7d/IbxYIg57HCv2tQsTJJtRT7Tm
mQUB0BQf+Hmk7v6dLXaqufB0dx7BTqkKhRJvSXKRyX1LopAB9VHiP8R8EKv/QYoO
Blw1EVvrzMaOb6wc7ElkCwdYl6oGSb3CTlSuhcOLsf6gkZGiCeWu3A==
-----END CERTIFICATE-----";
    }
}
