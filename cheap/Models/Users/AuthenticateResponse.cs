namespace cheap.Models.Users;

public class AuthenticateResponse
{
    public AuthenticateResponse()
    {
    }

    public AuthenticateResponse(UserModel user, string accessToken, string refreshToken)
    {
        Id = user.Id;
        Username = user.Username;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Success = true;
    }

    public Guid Id { get; set; }
    public String? Username { get; set; }
    public String? AccessToken { get; set; }
    public String? RefreshToken { get; set; }
    public Boolean Success { get; set; }
}