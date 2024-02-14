using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cheap.Entities;

public class UserPreference
{
    [Key] public virtual Guid Id { get; set; }
    [Required] [ForeignKey("UserId")] public Guid UserId { get; set; }
    public double SearchRadius { get; set; }
    public Boolean UseMiles { get; set; }
    public Boolean UseKilometers { get; set; }
}