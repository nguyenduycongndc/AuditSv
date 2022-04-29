using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audit_service.DataAccess
{
    public static class JwtTokenCm
    {
        public static JwtSecurityToken GetTokenInfo(string token, string secret)
        {
            SecurityToken validatedToken;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out validatedToken);
                return (JwtSecurityToken)validatedToken;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
