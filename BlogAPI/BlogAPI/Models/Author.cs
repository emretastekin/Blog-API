using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Range(1,100)]
        public short Age { get; set; }

        public List<Post>? Posts { get; set; }
	}
}

