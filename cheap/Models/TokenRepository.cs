namespace cheap.Models;

public class TokenRepository
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public String RefreshToken { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime Expiration { get; set; }
    public Boolean Expired { get; set; }
}