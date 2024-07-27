using System.Text.Json.Serialization;

namespace MockAPI.AppData.Entities
{
    // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
    public class Role
    {
        public int Id { get; set; }
        public required string RoleCode { get; set; }
        public required string RoleName { get; set; }
        [JsonIgnore] public ICollection<User>? Users { get; set; } = []; // to prevent cycling in json serialization!
    }
}