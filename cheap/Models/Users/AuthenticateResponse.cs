namespace cheap.Models.Users;

public class AuthenticateResponse
{
    public AuthenticateResponse()
    {
    }

    public AuthenticateResponse(UserModel user, string token)
    {
        Id = user.Id;
        Username = user.Username;
        Token = token;
        Success = true;
    }

    public Guid Id { get; set; }
    public String? Username { get; set; }
    public String? Token { get; set; }
    public Boolean Success { get; set; }
}