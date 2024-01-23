using cheap.Models.Users;

namespace cheap.Models;

public class RecordModel
{
    public virtual Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required LocationModel? Location { get; set; }
    public required ItemModel? Item { get; set; }
}