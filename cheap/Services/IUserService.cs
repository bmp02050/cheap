using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using cheap.Entities;
using cheap.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace cheap.Services;

public interface IUserService
{
    Task<User> GetById(Guid id);
    Task<User> Create(User user, string password);
    Task<User> Update(User user, string password = null);
    Task Delete(Guid id);
}

public class UserService : IUserService
{
    private readonly Context Context;
    private readonly Jwt _jwt;


    public UserService(Context context, IOptions<Jwt> jwtSettings)
    {
        Context = context;
        _jwt = jwtSettings.Value;
    }
    
    public async Task<User> GetById(Guid id)
    {
        return await Context.Users.Where(x => x.Id == id)
            .Include(x => x.Records)
            .ThenInclude(x => x.Item)
            .Include(x => x.Records)
            .ThenInclude(x => x.Item)
            .FirstAsync();
    }

    public async Task<User> Create(User user, string password)
    {
        // validation
        if (string.IsNullOrWhiteSpace(password))
            throw new Exception("Password is required");

        if (Context.Users.Any(x => x.Username == user.Username))
            throw new Exception("Username \"" + user.Username + "\" is unavailable");

        user.VerifiedEmail = false;
        CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        return user;
    }

    public async Task<User> Update(User userParam, string password = null)
    {
        var user = await GetById(userParam.Id);

        if (user == null)
            throw new Exception("User not found");

        // Update username if it has changed and is not already taken
        if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
        {
            if (Context.Users.Any(x => x.Username == userParam.Username))
                throw new Exception("Username " + userParam.Username + " is already taken");
            user.Username = userParam.Username;
        }

        // Use reflection to update properties dynamically
        foreach (var propertyInfo in userParam.GetType().GetProperties())
        {
            // Check if the property is not null
            var newValue = propertyInfo.GetValue(userParam);
            var currentValue = propertyInfo.GetValue(user);

            if (newValue != null && !newValue.Equals(currentValue))
            {
                propertyInfo.SetValue(user, newValue);
            }
        }

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(password))
        {
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        await Context.SaveChangesAsync();
        return user;
    }

    public async Task Delete(Guid id)
    {
        var user = await Context.Users.FindAsync(id);
        if (user != null)
        {
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
        }
    }

    
    // private helper methods

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
    
}