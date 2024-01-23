using System.ComponentModel.DataAnnotations;

namespace cheap.Entities;

public class RegistrationInviteToken 
{
    [Key] public virtual Guid Id { get; set; }
    [Required] public string Token { get; set; }
    [Required] public Guid UserId { get; set; }
    [Required] public Boolean Used { get; set; }
    [Required]public DateTime Expiration { get; set; }
}

