using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cheap.Entities;

public class Record
{
    [Key] public virtual Guid Id { get; set; }
    [Required] public Guid UserId { get; set; }
    public virtual User User { get; set; }
    [ForeignKey("LocationId")] public Guid LocationId { get; set; }
    public virtual Location? Location { get; set; }
    [ForeignKey("ItemId")] public Guid ItemId { get; set; }
    public virtual Item? Item { get; set; }
}