using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace EsiaSample
{
    /// <summary>
    /// Собственная реализация проверки маркера доступа.
    /// </summary>
    public class CustomSecurityTokenValidator : JwtSecurityTokenHandler
    {
        protected override string ValidateIssuer(string issuer, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
            => validationParameters.ValidIssuer;
    }
}
