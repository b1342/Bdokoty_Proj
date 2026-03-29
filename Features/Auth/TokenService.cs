using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WorkshowcaseApi.Common.Constants;
using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Features.Auth;

public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string GenerateToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var secret = GetRequiredValue("Jwt:Secret");
        var issuer = GetRequiredValue("Jwt:Issuer");
        var audience = GetRequiredValue("Jwt:Audience");
        var expiryMinutesRaw = GetRequiredValue("Jwt:ExpiryMinutes");

        if (!int.TryParse(expiryMinutesRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var expiryMinutes) ||
            expiryMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:ExpiryMinutes must be a positive integer.");
        }

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(AppClaimTypes.UserId, user.Id.ToString()),
            new(AppClaimTypes.Email, user.Email),
            new(AppClaimTypes.Role, user.UserType.ToString()),
            new(AppClaimTypes.FullName, user.FullName),
            new(AppClaimTypes.Status, user.Status.ToString()),
            new(AppClaimTypes.CreatedAt, user.CreatedAt.ToString("O", CultureInfo.InvariantCulture))
        };

        if (!string.IsNullOrWhiteSpace(user.Phone))
        {
            claims.Add(new Claim(AppClaimTypes.Phone, user.Phone));
        }

        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            claims.Add(new Claim(AppClaimTypes.ProfileImageUrl, user.ProfileImageUrl));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = now.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetRequiredValue(string key)
    {
        var value = _configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing required configuration value: {key}");
        }

        return value;
    }
}
