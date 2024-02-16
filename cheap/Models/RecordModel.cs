using cheap.Models.Users;
using Newtonsoft.Json;

namespace cheap.Models;

public class RecordModel
{
    [JsonProperty("id")] public Guid? Id { get; set; }
    [JsonProperty("userId")] public Guid UserId { get; set; }
    [JsonProperty("location")] public LocationModel? Location { get; set; }
    [JsonProperty("item")] public ItemModel? Item { get; set; }
}