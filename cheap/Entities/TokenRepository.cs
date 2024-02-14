using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cheap.Entities;

public class TokenRepository
{
    [Key] public virtual Guid Id { get; set; }
    [ForeignKey("UserId")] [Required] public Guid UserId { get; set; }
    [Required] public String RefreshToken { get; set; }
    [Required] public DateTime CreatedOn { get; set; }
    [Required] public DateTime Expiration { get; set; }
    [Required] public Boolean Expired { get; set; }
    
    
}