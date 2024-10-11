using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;

public static class JwtUtil
{
    private static readonly string JwtSecret = Env.GetString("JWT_SECRET");

    // Generate a JWT token
    public static string GenerateJwt(string userId, string email, string profileId, bool verified)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(JwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim("Id", userId),
                    new Claim("Email", email),
                    new Claim("ProfileId", profileId),
                    new Claim("Verified", verified.ToString()),
                ]
            ),
            Expires = DateTime.UtcNow.AddDays(3), // Token expiry time set to 3 days
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // Verify and parse the JWT token
    public static IDictionary<string, object>? VerifyJwt(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(JwtSecret);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // Skip issuer validation
                ValidateAudience = false, // Skip audience validation
                ClockSkew =
                    TimeSpan.Zero // Remove clock skew
                ,
            };

            var principal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out SecurityToken validatedToken
            );

            // Ensure the token is a valid JWT
            if (
                validatedToken is JwtSecurityToken jwtToken
                && jwtToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                // Extract claims from the token
                var claims = jwtToken.Claims.ToDictionary(
                    claim => claim.Type,
                    claim => (object)claim.Value
                );

                // Convert "Verified" claim to boolean
                if (
                    claims.TryGetValue("Verified", out object? value)
                    && bool.TryParse(value.ToString(), out bool Verified)
                )
                {
                    claims["Verified"] = Verified;
                }

                return claims;
            }
        }
        catch
        {
            // Token validation failed
            return null;
        }

        return null;
    }
}
