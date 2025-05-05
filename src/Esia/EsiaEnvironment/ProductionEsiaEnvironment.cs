using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;

/// <summary>
/// Описание продуктивной среды ЕСИА.
/// </summary>
public class ProductionEsiaEnvironment : IEsiaEnvironment
{
    /// <inheritdoc />
    public X509Certificate2 EsiaCertificate
    {
        get
        {
            var certificateBytes = Encoding.UTF8.GetBytes(EsiaCertificates.ProductionCertificate);
            return X509CertificateLoader.LoadCertificate(certificateBytes);
        }
    }

    /// <inheritdoc />
    public string Host => "https://esia.gosuslugi.ru";
    
    /// <inheritdoc />
    public string BackchannelUri => Host;

    /// <inheritdoc />
    public string AuthorizationEndpoint => Host + "/aas/oauth2/v2/ac";

    /// <inheritdoc />
    public string TokenEndpoint => BackchannelUri + "/aas/oauth2/v3/te";

    /// <inheritdoc />
    public string LogoutEndpoint => Host + "/idp/ext/Logout";

    /// <inheritdoc />
    public string RestPersonsEndpoint => Host + "/rs/prns/";

    /// <inheritdoc />
    public string Issuer => "http://esia.gosuslugi.ru/";
}