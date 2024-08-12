using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required]
        public string Article { get; set; } = "";

        public DateTime CommentedDate { get; set; }

        public int LikeCount { get; set; }

        public int DislikeCount { get; set; }

        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public string AuthorId { get; set; } = "";

        [ForeignKey(nameof(AuthorId))]
        public Author? Author { get; set; }

        [JsonIgnore]
        public Post? Post { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(CommentOfComment))]
        public int? CommentOfCommentId { get; set; }

        [JsonIgnore]
        public Comment? CommentOfComment { get; set; }

        

        [JsonIgnore]
        public List<Comment>? CommentAnswers { get; set; } = new List<Comment>();

        [JsonIgnore]
        public List<LikeDislike>? LikeDislikes { get; set; }


    }

}
