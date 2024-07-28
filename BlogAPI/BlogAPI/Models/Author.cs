using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class Author
    {
        [Key]
        public string Id { get; set; } = "";

        [ForeignKey(nameof(Id))]
        public ApplicationUser? ApplicationUser { get; set; }

        public string? Bio { get; set; }

        public string? Department { get; set; }

        [Range(1, 100)]
        public short Age { get; set; }

        [JsonIgnore]
        public List<Post>? Posts { get; set; }
    }
}
