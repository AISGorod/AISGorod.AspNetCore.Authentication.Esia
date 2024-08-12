using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace EsiaSample;

/// <summary>
/// Собственная реализация проверки маркера доступа.
/// НЕ для использования в проиышленной среде!
/// </summary>
[Obsolete("NOT FOR PRODUCTION USE!")]
public class CustomSecurityTokenValidator : JwtSecurityTokenHandler
{
    protected override string ValidateIssuer(string issuer, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
        => validationParameters.ValidIssuer;
}