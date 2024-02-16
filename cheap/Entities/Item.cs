using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cheap.Entities;

public class Item
{
    [Key] public virtual Guid Id { get; set; }
    [ForeignKey("RecordId")] public Guid RecordId { get; set; }
    [Required] [MaxLength(100)] public String? Name { get; set; }
    [Required] [MaxLength(500)] public String? Description { get; set; }
    [Required] public Double UnitPrice { get; set; }
    [Required] public Double Cost { get; set; }
    [Required] public Double Quantity { get; set; }
    [Required] [MaxLength(30)] public String? Barcode { get; set; }
    [Required] public String? ImageData { get; set; }
}