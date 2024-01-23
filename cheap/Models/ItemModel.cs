using System.ComponentModel.DataAnnotations;

namespace cheap.Models.Users;

public class ItemModel
{
    public Guid Id { get; set; }
    public Guid RecordId { get; set; }
    [MaxLength(100)] public String? Name { get; set; }
    [MaxLength(500)] public String? Description { get; set; }
    public Double UnitPrice { get; set; }
    public Double Cost { get; set; }
    public Double Quantity { get; set; }
    [MaxLength(30)] public String? Barcode { get; set; }
    public Byte[]? ImageData { get; set; }
}