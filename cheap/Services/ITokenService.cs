using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using cheap.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace cheap.Services;

public interface ITokenService
{
    Task<String> GenerateRegistrationInvitationTokenAsync(User user);
    Task<Boolean> ConfirmRegistrationAsync(Guid userId, String token);
}

public class TokenService : ITokenService
{
    private readonly Context Context;
    private readonly Jwt _jwt;


    public TokenService(Context context, IOptions<Jwt> jwtSettings)
    {
        Context = context;
        _jwt = jwtSettings.Value;
    }
    public async Task<String> GenerateRegistrationInvitationTokenAsync(User user)
    {
        var tokenData = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenData);
        }
        var tokenString = Convert.ToBase64String(tokenData);
        var token = new RegistrationInviteToken()
        {
            UserId = user.Id,
            Token = tokenString,
            Expiration = DateTime.UtcNow.AddMinutes(10),
            Used = false
        };
        var newToken = await Context.RegistrationInviteTokens.AddAsync(token);
        await Context.SaveChangesAsync();
        return newToken.Entity.Token;
    }

    public async Task<bool> ConfirmRegistrationAsync(Guid userId, string token)
    {
        var existingToken = await Context.RegistrationInviteTokens.FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token);
        var now = DateTime.UtcNow;
        if (now <= existingToken?.Expiration)
        {
            existingToken.Used = true;
            Context.RegistrationInviteTokens.Update(existingToken);
            await Context.SaveChangesAsync();
            return true;
        }

        return false;

    }
    private (string authToken, string refreshToken) GetTokens(User user)
    {
        if (_jwt.Key is null)
            throw new Exception("Missing JWT Key");
        if (user?.Email is null || user?.Username is null)
            throw new Exception("Username or email is null");
        
        var issuer = _jwt.Issuer;
        var audience = _jwt.Audience;
        var key = Encoding.ASCII.GetBytes(_jwt.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var authToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        // Generating refresh token
        var refreshToken = GenerateRefreshToken();

        return (authToken, refreshToken);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}