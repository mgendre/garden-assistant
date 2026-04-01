using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using GardenAssistant.Services.Interfaces;

namespace GardenAssistant.Services;

public class AuthService(AppDbContext db, IConfiguration configuration) : IAuthService
{
    public async Task<(string accessToken, string refreshToken)> GetDevelopmentTokenAsync()
    {
        var user = await db.Users.OrderBy(u => u.Id).FirstAsync();
        return await CreateTokensAsync(user);
    }

    public async Task<(string accessToken, string refreshToken)?> RefreshAsync(string refreshToken)
    {
        var result = await db.RefreshTokens
            .Where(rt => rt.Token == refreshToken && rt.ExpiresAt > DateTime.UtcNow)
            .Join(db.Users, rt => rt.UserId, u => u.Id, (rt, u) => new { Token = rt, User = u })
            .FirstOrDefaultAsync();

        if (result is null)
        {
            return null;
        }

        db.RefreshTokens.Remove(result.Token);
        await db.SaveChangesAsync();

        return await CreateTokensAsync(result.User);
    }
    
    private async Task<(string accessToken, string refreshToken)> CreateTokensAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateAndStoreRefreshTokenAsync(user.Id);
        return (accessToken, refreshToken);
    }

    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var accessTokenMinutes = configuration.GetValue<int>("Jwt:AccessTokenMinutes");

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(accessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateAndStoreRefreshTokenAsync(Guid userId)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(tokenBytes);

        var refreshTokenDays = configuration.GetValue<int>("Jwt:RefreshTokenDays");

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        return token;
    }
}
