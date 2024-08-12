using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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

        [StringLength(500)]
        public string? CoverImageUrl { get; set; } // Resim URL'si veya yolu

        [ForeignKey(nameof(AuthorId))]
        public Author? Author { get; set; }


        [JsonIgnore]
        public List<Comment>? Comments { get; set; }

        [JsonIgnore]
        public List<Favorite>? Favorites { get; set; }

        [JsonIgnore]
        public List<LikeDislike>? LikeDislikes { get; set; }

        public short? CategoryID { get; set; }  // Category ile ilişki

        [JsonIgnore]
        [ForeignKey(nameof(CategoryID))]
        public Category? Category { get; set; }  // Category nesnesi

        public short? SubCategoryID { get; set; }  // SubCategory ile ilişki

        [JsonIgnore]
        [ForeignKey(nameof(SubCategoryID))]
        public SubCategory? SubCategory { get; set; }  // SubCategory nesnesi





    }
}
