using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

namespace MockAPI.AppData.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Username { get; set; }
        [EmailAddress] public required string Email { get; set; }
        [JsonIgnore] public byte[] PasswordHash { get; set; } = []!;
        [JsonIgnore] public byte[] PasswordSalt { get; set; } = []!;
        public ICollection<Role>? Roles { get; set; } = [];
        public Address? Address { get; set; }
        public string? Phone { get; set; }
        public string? WebSite { get; set; }
        public Company? Company { get; set; }        
    }
}