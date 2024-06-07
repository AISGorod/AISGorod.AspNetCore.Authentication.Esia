using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace EsiaNet8Sample;

/// <summary>
/// Собственная реализация проверки маркера доступа.
/// НЕ для использования в промышленной среде!
/// </summary>
[Obsolete("NOT FOR PRODUCTION USE!")]
public class CustomSecurityTokenValidator : JwtSecurityTokenHandler
{
    protected override string ValidateIssuer(string issuer, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
        => validationParameters.ValidIssuer;
}