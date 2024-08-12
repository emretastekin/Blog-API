using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Models
{
	public class LikeDislike
	{
		public int Id { get; set; }

		public int PostId { get; set; }

        public int? CommentId { get; set; }  //Her postun yorumu olmak zorunda değildir.


        public string AuthorId { get; set; } = "";

		[ForeignKey(nameof(PostId))]
		public Post? Post { get; set; }

		[ForeignKey(nameof(AuthorId))]
		public Author? Author { get; set; }

		[ForeignKey(nameof(CommentId))]
		public Comment? Comment { get; set; }

		public bool IsLiked { get; set; }
	}
}

