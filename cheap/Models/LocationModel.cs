namespace cheap.Models;

public class LocationModel
{
    public Guid Id { get; set; }
    public Guid RecordId { get; set; }
    public Decimal Latitude { get; set; }
    public Decimal Longitude { get; set; }
    public String? LocationName { get; set; }
}