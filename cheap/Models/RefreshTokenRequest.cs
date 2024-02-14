namespace cheap.Models;

public class RefreshTokenRequest
{
    public required Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}