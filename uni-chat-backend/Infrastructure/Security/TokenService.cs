using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Configuration;

namespace uni_chat_backend.Infrastructure.Security;

public class TokenService(JwtSettings jwt, RefreshTokenSettings refresh)
{
    private readonly JwtSettings _jwt = jwt;
    private readonly RefreshTokenSettings _refresh = refresh;

    public string GenerateAccessToken(User user)
    {
        var keyString = _jwt.Key
            ?? throw new Exception("JWT Key no configurada");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(keyString)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(randomBytes),
            UserId = userId,
            Expires = DateTime.UtcNow.AddDays(_refresh.ExpireDays),
            IsRevoked = false
        };
    }
}