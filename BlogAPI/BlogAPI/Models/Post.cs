using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace BlogAPI.Models
{
    public class Post
    {
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public DateTime PostedDate { get; set; }

        public int LikeCount { get; set; }

        public int DislikeCount { get; set; }

        public string AuthorId { get; set; } = "";

        [ForeignKey(nameof(AuthorId))]
        public Author? Author { get; set; }


        public List<Comment>? Comments { get; set; }
    }
}
