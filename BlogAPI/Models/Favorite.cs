using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }
        public string AuthorId { get; set; } = "";
        public int PostId { get; set; }
        

        [ForeignKey(nameof(AuthorId))]
        public Author? Author { get; set; }


        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }

        
    }
}
