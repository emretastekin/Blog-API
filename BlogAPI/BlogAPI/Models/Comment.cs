using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

		public Post? Post { get; set; }
	}

}

