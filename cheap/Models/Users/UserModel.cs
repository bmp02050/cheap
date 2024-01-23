using System.ComponentModel.DataAnnotations;

namespace cheap.Models.Users;

public class UserModel
{
    public Guid Id { get; set; }
    [Required] public String? Username { get; set; }
    
}