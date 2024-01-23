using System.ComponentModel.DataAnnotations;

namespace cheap.Models.Users;

public class AuthenticateModel
{
    public String? Username { get; set; }
    public String? Email { get; set; }
    [Required] public String? Password { get; set; }
}