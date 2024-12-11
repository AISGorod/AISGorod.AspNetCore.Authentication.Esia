using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;

/// <summary>
/// Описание продуктивной среды ЕСИА.
/// </summary>
public class ProductionEsiaEnvironment : IEsiaEnvironment
{
	/// <summary>
	/// Сертификат среды ЕСИА.
	/// </summary>
	public X509Certificate2 EsiaCertificate
	{
		get
		{
			var certificateBytes = Encoding.UTF8.GetBytes(EsiaCertificates.ProductionCertificate);
			return X509CertificateLoader.LoadCertificate(certificateBytes);
		}
	}

	/// <summary>
	/// Базовый URL для запросов.
	/// </summary>
	public string Host => "https://esia.gosuslugi.ru";

	/// <summary>
	/// Endpoint для получения авторизационного кода.
	/// </summary>
	public string AuthorizationEndpoint => Host + "/aas/oauth2/ac";

	/// <summary>
	/// Endpoint для получения маркера доступа и(или) маркера идентификации.
	/// </summary>
	public string TokenEndpoint => Host + "/aas/oauth2/te";

	/// <summary>
	/// Endpoint для логаута.
	/// </summary>
	public string LogoutEndpoint => Host + "/idp/ext/Logout";

	/// <summary>
	/// Базовый URL для REST-сервиса персональных данных.
	/// </summary>
	public string RestPersonsEndpoint => Host + "/rs/prns/";

	/// <summary>
	/// Issuer маркеров доступа.
	/// </summary>
	public string Issuer => "http://esia.gosuslugi.ru/";
}