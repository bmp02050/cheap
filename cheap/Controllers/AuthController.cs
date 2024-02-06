using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using cheap.Entities;
using cheap.Models;
using cheap.Models.Users;
using cheap.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace cheap.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly Context Context;
    private readonly Jwt _jwt;

    public AuthController(TokenService tokenService, Context context, IOptions<Jwt> jwtSettings)
    {
        _tokenService = tokenService;
        Context = context;
        _jwt = jwtSettings.Value;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel request)
    {
        // Authenticate user (validate credentials, etc.)
        var user = await AuthenticateUserAsync(request.Username, request.Password);

        if (user == null)
            return Unauthorized();

        // Generate tokens
        var (accessToken, refreshToken) = _tokenService.GetTokens(user);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] RefreshTokenRequest request)
    {
        // Validate the refresh token (e.g., check against stored tokens)
        var isValid = ValidateRefreshToken(request.RefreshToken);

        if (!isValid)
            return Unauthorized();

        // Generate a new access token
        var newAccessToken = _tokenService.GenerateAccessTokenFromRefreshToken(request.RefreshToken);

        if (newAccessToken == null)
            return Unauthorized();
        
        var token = GetToken(user);
        return new AuthenticateResponse(new UserModel()
        {
            Id = user.Id,
            Username = user.Username,
        }, token.accessToken, token.refreshToken);
        return Ok(new { AccessToken = newAccessToken });
    }

    private async Task<User?> AuthenticateUserAsync(string username, string password)
    {
        var user = await Context.Users.FirstOrDefaultAsync(x => x.Username == username);

        // check if username exists
        if (user == null)
            return null;

        // check if password is correct
        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            return null;

        if (!user.VerifiedEmail)
            return null;
        
        return user;
    }

    private bool ValidateRefreshToken(string refreshToken)
    {
        // Your refresh token validation logic here
        // Check if the refresh token is valid (e.g., compare against stored tokens)
        // Return true if valid, false otherwise
    }

    private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
        if (storedHash.Length != 64)
            throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
        if (storedSalt.Length != 128)
            throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return !computedHash.Where((t, i) => t != storedHash[i]).Any();
    }
    private (string accessToken, string refreshToken) GetToken(User user)
    {
        var issuer = _jwt.Issuer;
        var audience = _jwt.Audience;
        var key = Encoding.ASCII.GetBytes
            (_jwt.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,
                    user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
        };
        var refreshToken = Guid.NewGuid().ToString(); // Generate a refresh token

        tokenDescriptor.Subject.AddClaim(new Claim("refresh_token", refreshToken));

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        return (accessToken, refreshToken);
    }
}