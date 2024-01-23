using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cheap.Entities;

public class Location
{
    [Key] public virtual Guid Id { get; set; }
    [ForeignKey("RecordId")] [Required] public Guid RecordId { get; set; }
    [Required] public Decimal Latitude { get; set; }
    [Required] public Decimal Longitude { get; set; }
    [Required] public String? LocationName { get; set; }
}