using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DevHabit.Api.DTO.Auth;
using DevHabit.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DevHabit.Api.Services;
public sealed class TokenProvider(IOptions<JwtAuthOptions> options)
{
    private readonly JwtAuthOptions _Jwtoptions = options.Value;

    public AccessTokenDto Create(TokenRequest tokenRequest)
    {
        return new AccessTokenDto(GenerateAccessToken(tokenRequest), GenerateRefreshToken());
    }

    private string GenerateAccessToken(TokenRequest tokenRequest)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwtoptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = [
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, tokenRequest.userId),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, tokenRequest.email),
            ..tokenRequest.Roles.Select(role => new Claim(ClaimTypes.Role, role))
            ];
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_Jwtoptions.ExpirationInMinutes),
            SigningCredentials = credentials,
            Audience = _Jwtoptions.Audience,
            Issuer = _Jwtoptions.Issuer,    
        };

        var handler = new JsonWebTokenHandler();
        string accessToken = handler.CreateToken(tokenDescriptor);
        return accessToken;
    }

    private static string GenerateRefreshToken()
    {

        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes);
    }
}
