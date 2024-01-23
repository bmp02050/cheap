using System.Security.Cryptography;
using cheap.Entities;
using Microsoft.EntityFrameworkCore;

namespace cheap.Services;

public interface ITokenService
{
    Task<String> GenerateRegistrationInvitationTokenAsync(User user);
    Task<Boolean> ConfirmRegistrationAsync(Guid userId, String token);
}

public class TokenService : ITokenService
{
    private readonly Context Context;


    public TokenService(Context context)
    {
        Context = context;
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
}