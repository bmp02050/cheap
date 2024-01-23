using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace cheap.Entities;

public class User
{
    [Key] public virtual Guid Id { get; set; }
    public required string? Username { get; set; }
    [IgnoreDataMember] public byte[]? PasswordHash { get; set; }
    [IgnoreDataMember] public byte[]? PasswordSalt { get; set; }
    [IgnoreDataMember] public required string? Email { get; set; }
    [IgnoreDataMember] public DateTime? CreateDate { get; set; }
    [IgnoreDataMember] public DateTime? UpdatedDate { get; set; }
    [IgnoreDataMember] public Boolean VerifiedEmail { get; set; }
    public virtual ICollection<Record>? Records { get; set; }
}